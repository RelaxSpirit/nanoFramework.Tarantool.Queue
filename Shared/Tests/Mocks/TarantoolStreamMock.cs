// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses _writer file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
#else
using System;
using System.Collections;
using System.Text;
#endif
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Dto;
using nanoFramework.MessagePack.Exceptions;
using nanoFramework.Tarantool.Client;
using nanoFramework.Tarantool.Model;
using nanoFramework.Tarantool.Model.Enums;
using nanoFramework.Tarantool.Model.Headers;
using nanoFramework.Tarantool.Model.Requests;
using nanoFramework.Tarantool.Model.Responses;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
using nanoFramework.Tarantool.Tests.Mocks.Data;

namespace nanoFramework.Tarantool.Tests.Mocks
{
    internal class TarantoolStreamMock : MemoryStream
    {
        private readonly byte[] _tarantoolHello = Encoding.UTF8.GetBytes(TarantoolQueueMockContext.TarantoolHelloString);
        private readonly object _processingLock = new object();
        private readonly object _responseLock = new object();
        private readonly Thread _processPackageThread;
        private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _newRequestEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _newResponseEvent = new ManualResetEvent(false);
        private readonly byte[] _adminPasswordScramble;
        private readonly System.Collections.Queue _requestQueue = new System.Collections.Queue();
        private readonly System.Collections.Queue _responseQueue = new System.Collections.Queue();
        private readonly WaitHandle[] _readHandles;
        private readonly ClientOptions _clientOptions;
        private readonly TarantoolQueueMockContext _context = TarantoolQueueMockContext.Instanse;

        private bool _isAdminSession = false;
        private bool _isSessionOpen = false;
        private bool _isStreamClose = false;

        internal TarantoolStreamMock(ClientOptions clientOptions) : base()
        {
            _clientOptions = clientOptions;

            if (!string.IsNullOrEmpty(_clientOptions.ConnectionOptions.Nodes[0].Uri.Password))
            {
                var greetings = new GreetingsResponse(_tarantoolHello);
                _adminPasswordScramble = AuthenticationRequest.GetScrable(greetings, TarantoolQueueMockContext.AdminPassword);
            }
            else
            {
                _adminPasswordScramble = new byte[0];
            }

            _readHandles = new WaitHandle[] { _exitEvent, _newResponseEvent };

            _processPackageThread = new Thread(ProcessPackages);
            _processPackageThread.Start();
        }

        private static QueueType GetQueueType(string queueTypeString)
        {
            switch (queueTypeString)
            {
                case "fifo":
                    return QueueType.Fifo;
                case "fifottl":
                    return QueueType.FifoTtl;
                case "limfifottl":
                    return QueueType.LimFifoTtl;
                case "utube":
                    return QueueType.Utube;
                case "utubettl":
                    return QueueType.UtubeTtl;
                default:
                    return QueueType.CustomTube;
            }
        }

        private static bool CompareArray(byte[] source, byte[] destination)
        {
            if (source.Length != destination.Length)
            {
                return false;
            }

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != destination[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static void AddPacketSize(MemoryStream writer, PacketSize packetLength)
        {
            writer.Seek(0, SeekOrigin.Begin);
            MessagePackSerializer.Serialize(packetLength, writer);
        }

        private static void WritePingResponseToStream(MemoryStream writer, RequestId requestId, PingRequest pingRequest)
        {
            MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.Ok, requestId, 1), writer);
            MessagePackSerializer.Serialize(new DataResponseMock(pingRequest), writer);
            AddPacketSize(writer, new PacketSize((uint)(writer.Position - Constants.PacketSizeBufferSize)));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_isStreamClose)
            {
                throw new ObjectDisposedException(nameof(TarantoolStreamMock));
            }

            if (!_isSessionOpen)
            {
                if (count >= _tarantoolHello.Length)
                {
                    Array.Copy(_tarantoolHello, 0, buffer, offset, _tarantoolHello.Length);
                    _isSessionOpen = true;
                    return _tarantoolHello.Length;
                }
                else
                {
                    throw new ArgumentException($"Actual: {count} but expected {_tarantoolHello.Length}", nameof(count));
                }
            }
            else
            {
                var eventRead = WaitHandle.WaitAny(_readHandles);

                switch (eventRead)
                {
                    case 1:
                        if (_responseQueue.Count > 0)
                        {
                            int i = 0;
                            for (; i < count; i++)
                            {
#nullable enable
                                object? responseByte = null;
                                lock (_responseLock)
                                {
                                    if (_responseQueue.Count > 0)
                                    {
                                        responseByte = _responseQueue.Dequeue();
                                    }
                                    else
                                    {
                                        responseByte = null;
                                    }
                                }
#nullable disable
                                if (responseByte != null)
                                {
                                    buffer[offset + i] = (byte)responseByte;
                                }
                                else
                                {
                                    return i;
                                }
                            }

                            lock (_responseLock)
                            {
                                if (_responseQueue.Count < 1)
                                {
                                    _newResponseEvent.Reset();
                                }
                            }

                            return i;
                        }
                        else
                        {
                            _newResponseEvent.Reset();
                            return 0;
                        }

                    default:
                        return 0;
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_isStreamClose)
            {
                throw new ObjectDisposedException(nameof(TarantoolStreamMock));
            }

            if (!_isSessionOpen)
            {
                throw new NotSupportedException("Session is not open");
            }

            int finishOffset = count - 1;
            while (finishOffset > offset)
            {
                var firstRead = MessagePackSerializer.Deserialize(typeof(PacketSize), buffer) ?? throw new OutOfMemoryException("Invalid packet size");

                var packetSize = (PacketSize)firstRead;
                offset += Constants.PacketSizeBufferSize;

                var arraySegment = new ArraySegment(buffer, offset, packetSize.Value);
                RequestHeader requestHeader = (RequestHeader)(TarantoolQueueMockContext.RequestHeaderConverter.Read(arraySegment) ?? throw new SerializationException("Error serialize RequestHeader"));

                switch (requestHeader.Code)
                {
                    case CommandCode.Auth:
                        EnqueueAuthRequest(requestHeader.RequestId, arraySegment);
                        break;
                    case CommandCode.Select:
                        EnqueueSelectRequest(requestHeader.RequestId, arraySegment);
                        break;
                    case CommandCode.Call:
                    case CommandCode.OldCall:
                        EnqueueCallRequest(requestHeader.RequestId, arraySegment);
                        break;
                    case CommandCode.Eval:
                        EnqueueEvalRequest(requestHeader.RequestId, arraySegment);
                        break;
                    case CommandCode.Ping:
                        EnqueuePingRequest(requestHeader.RequestId, arraySegment);
                        break;
                    default:
                        throw new NotImplementedException($"Process request command code {requestHeader.Code} not implemented");
                }

                offset += (int)packetSize.Value;

                _newRequestEvent.Set();
            }
        }

        public override void Flush()
        {
            if (_isStreamClose)
            {
                throw new ObjectDisposedException(nameof(TarantoolStreamMock));
            }

            base.Flush();
        }

        public override void Close()
        {
            if (!_isStreamClose)
            {
                _exitEvent.Set();
                _isSessionOpen = false;
                _requestQueue.Clear();
                _responseQueue.Clear();

                base.Close();
            }

            _isStreamClose = true;
        }

#nullable enable
        private void ProcessPackages()
        {
            var handles = new WaitHandle[] { _exitEvent, _newRequestEvent };

            while (!_isStreamClose)
            {
                switch (WaitHandle.WaitAny(handles))
                {
                    case 0:
                        return;
                    case 1:

                        while (_requestQueue.Count > 0)
                        {
                            object? request = null;
                            lock (_processingLock)
                            {
                                if (_requestQueue.Count > 0)
                                {
                                    request = _requestQueue.Dequeue();
                                }
                            }

                            if (request != null)
                            {
                                EnqueueMockResponse((DictionaryEntry)request);
                            }
                        }

                        _newRequestEvent.Reset();

                        break;
                }
            }
        }
#nullable disable

        private void EnqueueAuthRequest(RequestId requestId, ArraySegment arraySegment)
        {
            AuthenticationRequest request = (AuthenticationRequest)(TarantoolQueueMockContext.AuthenticationPacketConverter.Read(arraySegment) ?? throw new SerializationException("Error serialize AuthenticationRequest"));

            lock (_processingLock)
            {
                _requestQueue.Enqueue(new DictionaryEntry(requestId, request));
            }
        }

        private void EnqueueSelectRequest(RequestId requestId, ArraySegment arraySegment)
        {
            SelectRequest request = (SelectRequest)(TarantoolQueueMockContext.SelectPacketConverter.Read(arraySegment) ?? throw new SerializationException("Error serialize SelectRequest"));

            lock (_processingLock)
            {
                _requestQueue.Enqueue(new DictionaryEntry(requestId, request));
            }
        }

        private void EnqueueCallRequest(RequestId requestId, ArraySegment arraySegment)
        {
            CallRequest request = (CallRequest)(TarantoolQueueMockContext.CallPacketConverter.Read(arraySegment) ?? throw new SerializationException("Error serialize CallRequest"));

            lock (_processingLock)
            {
                _requestQueue.Enqueue(new DictionaryEntry(requestId, request));
            }
        }

        private void EnqueueEvalRequest(RequestId requestId, ArraySegment arraySegment)
        {
            EvalRequest request = (EvalRequest)(TarantoolQueueMockContext.EvalPacketConverter.Read(arraySegment) ?? throw new SerializationException("Error serialize EvalRequest"));

            lock (_processingLock)
            {
                _requestQueue.Enqueue(new DictionaryEntry(requestId, request));
            }
        }

        private void EnqueuePingRequest(RequestId requestId, ArraySegment arraySegment)
        {
            PingRequest request = (PingRequest)(TarantoolQueueMockContext.PingPacketConverter.Read(arraySegment) ?? throw new SerializationException("Error serialize PingRequest"));

            lock (_processingLock)
            {
                _requestQueue.Enqueue(new DictionaryEntry(requestId, request));
            }
        }

        private void EnqueueMockResponse(DictionaryEntry request)
        {
            using (MemoryStream writer = new MemoryStream())
            {
                writer.Seek(Constants.PacketSizeBufferSize, SeekOrigin.Begin);
                var requestId = (RequestId)request.Key;
                try
                {
                    if (request.Value is AuthenticationRequest authentication)
                    {
                        WriteAuthenticationResponseToStream(writer, requestId, authentication);
                    }
                    else
                    {
                        if (request.Value is SelectRequest select)
                        {
                            WriteSelectResponseToStream(writer, requestId, select);
                        }
                        else
                        {
                            if (request.Value is CallRequest call)
                            {
                                WriteCallResponseToStream(writer, requestId, call);
                            }
                            else
                            {
                                if (request.Value is EvalRequest eval)
                                {
                                    WriteEvalResponseToStream(writer, requestId, eval);
                                }
                                else
                                {
                                    if (request.Value is PingRequest ping)
                                    {
                                        WritePingResponseToStream(writer, requestId, ping);
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Unknown request type");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    writer.SetLength(0);
                    writer.Seek(Constants.PacketSizeBufferSize, SeekOrigin.Begin);
                    MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.ErrorMask, requestId, 1), writer);
                    MessagePackSerializer.Serialize(new ErrorResponse($"{e}"), writer);
                    AddPacketSize(writer, new PacketSize((uint)(writer.Position - Constants.PacketSizeBufferSize)));
                }
                finally
                {
                    writer.Seek(0, SeekOrigin.Begin);

                    lock (_responseLock)
                    {
                        int b = -1;
                        while ((b = writer.ReadByte()) > -1)
                        {
                            _responseQueue.Enqueue((byte)b);
                        }
                    }

                    if (_responseQueue.Count > 0)
                    {
                        _newResponseEvent.Set();
                    }
                }
            }
        }

        private void WriteAuthenticationResponseToStream(MemoryStream writer, RequestId requestId, AuthenticationRequest authenticationRequest)
        {
            if (authenticationRequest.Username == TarantoolQueueMockContext.AdminUserName && CompareArray(_adminPasswordScramble, authenticationRequest.Scramble))
            {
                MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.Ok, requestId, 1), writer);
                MessagePackSerializer.Serialize(new EmptyResponseMock(), writer);
                _isAdminSession = true;
            }
            else
            {
                MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.ErrorMask, requestId, 1), writer);
                MessagePackSerializer.Serialize(new ErrorResponse("User not found or supplied credentials are invalid."), writer);
            }

            AddPacketSize(writer, new PacketSize((uint)(writer.Position - Constants.PacketSizeBufferSize)));
        }

        private void WriteSelectResponseToStream(MemoryStream writer, RequestId requestId, SelectRequest selectRequest)
        {
            MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.Ok, requestId, 1), writer);

            switch (selectRequest.SpaceId)
            {
                case Schema.VSpace:
                    if (selectRequest.SelectKey != null)
                    {
                        MessagePackSerializer.Serialize(new DataResponseMock(new SpaceMock[] { _context.Spaces[1] }), writer);
                    }
                    else
                    {
                        MessagePackSerializer.Serialize(new DataResponseMock(_context.Spaces), writer);
                    }

                    break;
                case Schema.VIndex:
                    if (selectRequest.SelectKey != null)
                    {
                        MessagePackSerializer.Serialize(new DataResponseMock(new IndexMock[] { _context.Indices[1] }), writer);
                    }
                    else
                    {
                        MessagePackSerializer.Serialize(new DataResponseMock(_context.Indices), writer);
                    }   
                    
                    break;
                case 2:
                    if (selectRequest.SelectKey.Length > 0)
                    {
                        var keyString = selectRequest.SelectKey[0].ToString();
                        if (_context.TubesTable.Contains(keyString) && _context.TubesTable[keyString] is QueueDriver queueDriver)
                        {
                            MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { TarantoolTuple.Create(keyString, queueDriver.Id, keyString, queueDriver.CreationOptions.QueueType, queueDriver.CreationOptions) }), writer);
                            break;
                        }
                    }
                    
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[0]), writer);

                    break;
                default:
                    throw new NotImplementedException($"Unknown SpaceId value = '{selectRequest.SpaceId}'");
            }

            AddPacketSize(writer, new PacketSize((uint)(writer.Position - Constants.PacketSizeBufferSize)));
        }

        private void WriteEvalResponseToStream(MemoryStream writer, RequestId requestId, EvalRequest evalRequest)
        {
            MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.Ok, requestId, 1), writer);

            string exp = evalRequest.Expression;

            if (exp.StartsWith("queue.create_tube"))
            {
                if (_isAdminSession)
                {
                    exp = "queue.create_tube";
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            switch (exp)
            {
                case "return queue._VERSION":
                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Create("1.4.4")), writer);
                    break;
                case "queue.create_tube":
                    var starTubeName = evalRequest.Expression.IndexOf('(', evalRequest.Expression.IndexOf(exp)) + 1;
                    var endTubeName = evalRequest.Expression.IndexOf(',', starTubeName);
                    var crudeTubeName = evalRequest.Expression.Substring(starTubeName, endTubeName - starTubeName).Trim();
                    var tubeName = crudeTubeName.Substring(crudeTubeName.IndexOf('\'') + 1, crudeTubeName.LastIndexOf('\'') - 1);

                    var typeStartIndex = evalRequest.Expression.IndexOf('\'', endTubeName) + 1;
                    var typeEndIndex = evalRequest.Expression.IndexOf('\'', typeStartIndex);
                    var tubeTypeString = evalRequest.Expression.Substring(typeStartIndex, typeEndIndex - typeStartIndex);
                    var creationOptions = TubeCreationOptions.GetTubeCreationOptions(GetQueueType(tubeTypeString));
                    creationOptions.IfNotExists = true;
                    creationOptions.Temporary = false;

                    var optionsStartIndex = evalRequest.Expression.IndexOf(',', typeEndIndex);
                    if (optionsStartIndex > -1)
                    {
                        optionsStartIndex = evalRequest.Expression.IndexOf('{', typeEndIndex + 1) + 1;
                        var optionsEndIndex = evalRequest.Expression.IndexOf('}', optionsStartIndex);

                        var options = evalRequest.Expression.Substring(optionsStartIndex, optionsEndIndex - optionsStartIndex).Split(',');
                        foreach (var option in options)
                        {
                            var keyValue = option.Split('=');

                            creationOptions.Add(keyValue[0].Trim(), ulong.Parse(keyValue[1].Trim()));
                        }
                    }

                    var queueDriver = new QueueDriver(TarantoolQueueMockContext.Instanse.TubesTable.Count, creationOptions);
                    TarantoolQueueMockContext.Instanse.TubesTable.Add(tubeName, queueDriver);
                    
                    TarantoolQueueMockContext.Instanse.queueStatistic.QueueTubesStatistic.Add(tubeName, queueDriver.QueueTubeStatistic);
                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Empty), writer);
                    break;
                default:
                    throw new NotImplementedException($"Unknown Expression = '{evalRequest.Expression}'");
            }

            AddPacketSize(writer, new PacketSize((uint)(writer.Position - Constants.PacketSizeBufferSize)));
        }

        private void WriteCallResponseToStream(MemoryStream writer, RequestId requestId, CallRequest callRequest)
        {
            var functionName = callRequest.FunctionName;
            string functionPath = string.Empty;
            string tubeName = string.Empty;
            QueueDriver queueDriver = null;
            TarantoolTuple responseTuple;

            if (functionName.StartsWith("queue.tube."))
            {
                var path = callRequest.FunctionName.Split(':');
                functionName = path[1];
                functionPath = path[0];

                int lastPointIndex = functionPath.LastIndexOf('.');
                if (lastPointIndex != -1)
                {
                    lastPointIndex++;
                    tubeName = functionPath.Substring(lastPointIndex, functionPath.Length - lastPointIndex);

                    if (TarantoolQueueMockContext.Instanse.TubesTable[tubeName] is QueueDriver driver)
                    {
                        queueDriver = driver;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException($"Tube name '{tubeName}' not find.");
                    }
                }
            }

            MessagePackSerializer.Serialize(new ResponseHeader(CommandCode.Ok, requestId, 1), writer);
            switch (functionName)
            {
                case "queue.identify":
                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Create("&Z\n\x8E`\xF8\x98A\xABiq\xCE\xF3\x02i}")), writer);
                    break;
                case "queue.statistics":
                    if (callRequest.Tuple != null && callRequest.Tuple.Length > 0 && callRequest.Tuple[0] is string statisticTubeName && TarantoolQueueMockContext.Instanse.queueStatistic.QueueTubesStatistic[statisticTubeName] is QueueTubeStatisticMock tubeStatisticMock)
                    {
                        MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Create(tubeStatisticMock)), writer);
                    }

                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Create(TarantoolQueueMockContext.Instanse.queueStatistic)), writer);
                    break;
                case "queue.state":
                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Create("RUNNING")), writer);
                    break;
                case "drop":
                    if (_isAdminSession)
                    {
                        TarantoolQueueMockContext.Instanse.TubesTable.Remove(tubeName);
                        TarantoolQueueMockContext.Instanse.queueStatistic.QueueTubesStatistic.Remove(tubeName);
                        MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Empty), writer);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    break;
                case "put":
                    responseTuple = queueDriver.Put(callRequest.Tuple[0], callRequest.Tuple.Length > 1 ? callRequest.Tuple[1] as Hashtable : null);
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "take":
                    responseTuple = queueDriver.Take(callRequest.Tuple.Length > 0 ? TimeSpan.FromSeconds(long.Parse(callRequest.Tuple[0].ToString())) : TimeSpan.MaxValue);
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "ack":
                    responseTuple = queueDriver.Ack(int.Parse(callRequest.Tuple[0].ToString()));
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "peek":
                    responseTuple = queueDriver.Peek(int.Parse(callRequest.Tuple[0].ToString()));
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "delete":
                    responseTuple = queueDriver.Delete(int.Parse(callRequest.Tuple[0].ToString()));
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "bury":
                    responseTuple = queueDriver.Bury(int.Parse(callRequest.Tuple[0].ToString()));
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "kick":
                    ulong count = queueDriver.Kick(int.Parse(callRequest.Tuple[0].ToString()));
                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Create(count)), writer);
                    break;
                case "release":
                    responseTuple = queueDriver.Release(int.Parse(callRequest.Tuple[0].ToString()), callRequest.Tuple.Length > 1 ? callRequest.Tuple[1] as Hashtable : null);
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                case "release_all":
                    queueDriver.ReleaseAll();
                    MessagePackSerializer.Serialize(new DataResponseMock(TarantoolTuple.Empty), writer);
                    break;
                case "touch":
                    responseTuple = queueDriver.Touch(int.Parse(callRequest.Tuple[0].ToString()), TimeSpan.FromSeconds(long.Parse(callRequest.Tuple[1].ToString())));
                    MessagePackSerializer.Serialize(new DataResponseMock(new TarantoolTuple[] { responseTuple }), writer);
                    break;
                default:
                    throw new NotImplementedException($"Unknown Expression = '{callRequest.FunctionName}'");
            }

            AddPacketSize(writer, new PacketSize((uint)(writer.Position - Constants.PacketSizeBufferSize)));
        }
    }
}
