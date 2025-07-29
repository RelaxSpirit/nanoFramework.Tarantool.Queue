// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses _writer file to you under the MIT license.

using System.Collections;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.Tarantool.Converters;
using nanoFramework.Tarantool.Model;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
using nanoFramework.Tarantool.Tests.Mocks.Converters;
using nanoFramework.Tarantool.Tests.Mocks.Data;

namespace nanoFramework.Tarantool.Tests.Mocks
{
    internal class TarantoolQueueMockContext
    {
        internal const string TarantoolHelloString = "Tarantool 3.4.0 (Binary) 790215f6-c2d8-427c-9bb9-687f71c7e18a  \nVcSniiMfX+m5JH+a+WSffyJjkjHYO4Ku+d6afsjIT68=                    ";
        internal const string AdminUserName = "testuser";
        internal const string AdminPassword = "test_password";
        internal static readonly RequestHeaderConverterMock RequestHeaderConverter = new RequestHeaderConverterMock();
        internal static readonly AuthenticationPacketConverterMock AuthenticationPacketConverter = new AuthenticationPacketConverterMock();
        internal static readonly SelectPacketConverterMock SelectPacketConverter = new SelectPacketConverterMock();
        internal static readonly CallPacketConverterMock CallPacketConverter = new CallPacketConverterMock();
        internal static readonly EvalPacketConverterMock EvalPacketConverter = new EvalPacketConverterMock();       
        internal static readonly PingPacketConverter PingPacketConverter = new PingPacketConverter();
        private static object _lockInstance = new object();
#nullable enable
        private static TarantoolQueueMockContext? _instance;
#nullable disable

        private TarantoolQueueMockContext()
        {
            ConverterContext.Add(typeof(BoxInfoMock), new BoxInfoMock.BoxInfoConverterMock());
            ConverterContext.Add(typeof(SpaceMock), new SpaceConverterMock());
            ConverterContext.Add(typeof(SpaceMock[]), new SimpleArrayConverter(typeof(SpaceMock)));
            ConverterContext.Add(typeof(SpaceFieldMock), new SpaceFieldConverterMock());
            ConverterContext.Add(typeof(SpaceFieldMock[]), new SimpleArrayConverter(typeof(SpaceFieldMock)));
            ConverterContext.Add(typeof(IndexPartMock), new IndexPartConverterMock());
            ConverterContext.Add(typeof(IndexPartMock[]), new SimpleArrayConverter(typeof(IndexPartMock)));
            ConverterContext.Add(typeof(IndexMock), new IndexConverterMock());
            ConverterContext.Add(typeof(IndexMock[]), new SimpleArrayConverter(typeof(IndexMock)));
            ConverterContext.Add(typeof(DataResponseMock), new ResponsePacketConverterMock());
            ConverterContext.Add(typeof(EmptyResponseMock), new EmptyResponseConverterMock());
            ConverterContext.Add(typeof(QueueStatisticMock), new QueueStatisticConverterMock());
            ConverterContext.Add(typeof(IndexCreationOptionsMock), new IndexCreationOptionsConverterMock());
            ConverterContext.Add(typeof(QueueTubeStatisticMock), new QueueTubeStatisticConverterMock());
            ConverterContext.Replace(typeof(QueueState), new QueueStateConverterMock());
            ConverterContext.Replace(typeof(TubeInfo), new TubeInfoConverterMock());
            ConverterContext.Replace(typeof(TubeTaskState), new TubeTaskStateConverterMock());
        }

        public static TarantoolQueueMockContext Instanse
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockInstance)
                    {
                        if (_instance == null)
                        {
                            _instance = new TarantoolQueueMockContext();
                        }
                    }
                }

                return _instance;
            }
        }

        internal Hashtable TubesTable { get; } = new Hashtable();

        internal BoxInfoMock TestBoxInfo { get; } = BoxInfoMock.GetBoxInfo();

        internal SpaceMock[] Spaces { get; } = new SpaceMock[]
        {
            new SpaceMock(1, 1, "_space", Model.Enums.StorageEngine.Memtx, new SpaceFieldMock[] { new SpaceFieldMock("test", Model.Enums.FieldType.Num) }),
            new SpaceMock(2, 5, "_queue", Model.Enums.StorageEngine.Memtx, new SpaceFieldMock[] { new SpaceFieldMock("tube", Model.Enums.FieldType.Unsigned), new SpaceFieldMock("tube_id", Model.Enums.FieldType.Unsigned), new SpaceFieldMock("space_name", Model.Enums.FieldType.String), new SpaceFieldMock("type", Model.Enums.FieldType.Str), new SpaceFieldMock("options", Model.Enums.FieldType.Any) })
        };

        internal IndexMock[] Indices { get; } = new IndexMock[]
        {
            new IndexMock(0, 1, "main_index", true, Model.Enums.IndexType.RTree, new IndexPartMock[] { new IndexPartMock(1, Model.Enums.FieldType.Unsigned) }),
            new IndexMock(0, 2, "tube", true, Model.Enums.IndexType.Tree, new IndexPartMock[] { new IndexPartMock(0, Model.Enums.FieldType.String), new IndexPartMock(1, Model.Enums.FieldType.Unsigned) })
        };

        internal QueueStatisticMock queueStatistic { get; } = new QueueStatisticMock();

        internal TarantoolStreamMock GetTarantoolStreamMock(ClientOptions clientOptions)
        {
            return new TarantoolStreamMock(clientOptions);
        }
    }
}
