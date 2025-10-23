// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using nanoFramework.Tarantool.Client.Interfaces;
using nanoFramework.Tarantool.Model;
using nanoFramework.Tarantool.Queue.Client.Interfaces;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
using static nanoFramework.Tarantool.Queue.Model.QueueTubeStatistic;

namespace nanoFramework.Tarantool.Queue.Client
{
    internal class Tube : ITube
    {
        private readonly IBox _box;
        private readonly string[] _callStringTable = new string[11];
        private readonly Type _taskIdType = typeof(ulong);
        private readonly Type _tubeTaskState = typeof(TubeTaskState);

        private enum CallName : int
        {
            Ack = 0,
            Bury = 1,
            Delete = 2,
            GetStatistics = 3,
            Kick = 4,
            Peek = 5,
            Put = 6,
            Release = 7,
            ReleaseAll = 8,
            Take = 9,
            Touch = 10
        }

        internal Tube(TubeInfo tubeInfo, IBox box, string require)
        {
            _box = box;
            Name = tubeInfo.Name;
            CreationOptions = tubeInfo.CreationOptions ?? throw new ArgumentNullException();
            TubeType = CreationOptions.QueueType;
            string driverCallPath = $"{require}.tube.{Name}:";

            for (int i = 0; i < _callStringTable.Length; i++)
            {
                CallName callName = (CallName)i;
                switch (callName)
                {
                    case CallName.Ack:
                        _callStringTable[i] = $"{driverCallPath}ack";
                        break;
                    case CallName.Bury:
                        _callStringTable[i] = $"{driverCallPath}bury";
                        break;
                    case CallName.Delete:
                        _callStringTable[i] = $"{driverCallPath}delete";
                        break;
                    case CallName.GetStatistics:
                        _callStringTable[i] = $"{require}.statistics";
                        break;
                    case CallName.Kick:
                        _callStringTable[i] = $"{driverCallPath}kick";
                        break;
                    case CallName.Peek:
                        _callStringTable[i] = $"{driverCallPath}peek";
                        break;
                    case CallName.Put:
                        _callStringTable[i] = $"{driverCallPath}put";
                        break;
                    case CallName.Release:
                        _callStringTable[i] = $"{driverCallPath}release";
                        break;
                    case CallName.ReleaseAll:
                        _callStringTable[i] = $"{driverCallPath}release_all";
                        break;
                    case CallName.Take:
                        _callStringTable[i] = $"{driverCallPath}take";
                        break;
                    case CallName.Touch:
                        _callStringTable[i] = $"{driverCallPath}touch";
                        break;
                }
            }
        }

        public string Name { get; }

        public TubeCreationOptions CreationOptions { get; }

        public QueueType TubeType { get; }

#nullable enable
        private static void CheckTubeType(QueueType tubeType, TubeOptions? tubePutOptions)
        {
            if (tubePutOptions != null && tubePutOptions.QueueType != tubeType)
            {
                throw new NotSupportedException($"Queue tube type {tubeType} das not supported putting option for queue tube type {tubePutOptions.QueueType}");
            }
        }

        public TubeTask Ack(Type taskDataType, ulong taskId)
        {
            return GetTubeTask(_callStringTable[(int)CallName.Ack], taskId, taskDataType);
        }

        public void Ack(ulong taskId)
        {
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.Ack], TarantoolTuple.Create(taskId));
        }

        public TubeTask Bury(Type taskDataType, ulong taskId)
        {
            return GetTubeTask(_callStringTable[(int)CallName.Bury], taskId, taskDataType);
        }

        public void Bury(ulong taskId)
        {
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.Bury], TarantoolTuple.Create(taskId));
        }

        public TubeTask Delete(Type taskDataType, ulong taskId)
        {
            return GetTubeTask(_callStringTable[(int)CallName.Delete], taskId, taskDataType);
        }

        public void Delete(ulong taskId)
        {
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.Delete], TarantoolTuple.Create(taskId));
        }

        public QueueTubeStatistic GetStatistics()
        {
            var responseData = _box.Call(_callStringTable[(int)CallName.GetStatistics], TarantoolTuple.Create(Name), TarantoolContext.Instance.GetTarantoolTupleType(typeof(QueueTubeStatistic)));
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple && tarantoolTuple[0] is QueueTubeStatistic queueTubeStatistic)
            {
                return queueTubeStatistic;
            }
            else
            {
                throw new Exception("Error getting tube statistic.");
            }
        }

        public ulong Kick(ulong count)
        {
            var responseData = _box.Call(_callStringTable[(int)CallName.Kick], TarantoolTuple.Create(count), TarantoolContext.Instance.GetTarantoolTupleType(typeof(ulong)));
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return (ulong)(tarantoolTuple[0] ?? 0);
            }
            else
            {
                return 0;
            }
        }

        public TubeTask Peek(Type taskDataType, ulong taskId)
        {
            return GetTubeTask(_callStringTable[(int)CallName.Peek], taskId, taskDataType);
        }

        public TubeTask Put([NotNull] object data, TubeOptions? opts = null)
        {
            CheckTubeType(TubeType, opts);

            var tupleArrayType = TarantoolContext.Instance.GetTarantoolTupleArrayType(TarantoolContext.Instance.GetTarantoolTupleType(_taskIdType, _tubeTaskState, data.GetType()));
            
            var responseData = opts != null ? _box.Call(_callStringTable[(int)CallName.Put], TarantoolTuple.Create(data, opts), tupleArrayType) : _box.Call(_callStringTable[(int)CallName.Put], TarantoolTuple.Create(data), tupleArrayType);
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return TubeTask.GetTubeTask(tarantoolTuple);
            }

            throw new Exception("Message is not put in Tarantool.Queue tube.");
        }

        public void PutWithEmptyResponse([NotNull] object data, TubeOptions? opts = null)
        {
            CheckTubeType(TubeType, opts);

            var parameters = opts != null ? TarantoolTuple.Create(data, opts) : TarantoolTuple.Create(data);
            
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.Put], parameters);
        }

        public TubeTask Release(Type taskDataType, ulong taskId, TubeOptions opts)
        {
            CheckTubeType(TubeType, opts);

            var tupleArrayType = TarantoolContext.Instance.GetTarantoolTupleArrayType(TarantoolContext.Instance.GetTarantoolTupleType(_taskIdType, _tubeTaskState, taskDataType));
            var responseData = _box.Call(_callStringTable[(int)CallName.Release], TarantoolTuple.Create(taskId, opts), tupleArrayType);
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return TubeTask.GetTubeTask(tarantoolTuple);
            }

            return default;
        }

        public void Release(ulong taskId, TubeOptions opts)
        {
            CheckTubeType(TubeType, opts);
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.Release], TarantoolTuple.Create(taskId, opts));
        }

        public void ReleaseAll()
        {
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.ReleaseAll], null);
        }

        public TubeTask Take(Type taskDataType)
        {
            var tupleArrayType = TarantoolContext.Instance.GetTarantoolTupleArrayType(TarantoolContext.Instance.GetTarantoolTupleType(_taskIdType, _tubeTaskState, taskDataType));
            var responseData = _box.Call(_callStringTable[(int)CallName.Take], tupleArrayType);
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return TubeTask.GetTubeTask(tarantoolTuple);
            }

            return default;
        }

        public TubeTask Take(Type taskDataType, TimeSpan timeout, TubeOptions? opts = null)
        {
            if (timeout == Timeout.InfiniteTimeSpan && opts == null)
            {
                return Take(taskDataType);
            }

            CheckTubeType(TubeType, opts);

            var tupleArrayType = TarantoolContext.Instance.GetTarantoolTupleArrayType(TarantoolContext.Instance.GetTarantoolTupleType(_taskIdType, _tubeTaskState, taskDataType));

            TarantoolTuple tuple;

            if (opts != null)
            {
                tuple = TarantoolTuple.Create((ulong)timeout.TotalSeconds, opts);
            }
            else
            {
                tuple = TarantoolTuple.Create((ulong)timeout.TotalSeconds);
            }

            var responseData = _box.Call(_callStringTable[(int)CallName.Take], tuple, tupleArrayType);
           
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return TubeTask.GetTubeTask(tarantoolTuple);
            }

            return default;
        }

        public TubeTask Touch(Type taskDataType, ulong taskId, TimeSpan delta)
        {
            var tupleArrayType = TarantoolContext.Instance.GetTarantoolTupleArrayType(TarantoolContext.Instance.GetTarantoolTupleType(_taskIdType, _tubeTaskState, taskDataType));
            var responseData = _box.Call(_callStringTable[(int)CallName.Touch], TarantoolTuple.Create(taskId, (long)delta.TotalSeconds), tupleArrayType);
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return TubeTask.GetTubeTask(tarantoolTuple);
            }

            return default;
        }

        public void Touch(ulong taskId, TimeSpan delta)
        {
            _box.CallWithEmptyResponse(_callStringTable[(int)CallName.Touch], TarantoolTuple.Create(taskId, (long)delta.TotalSeconds));
        }

        private TubeTask GetTubeTask(string callExpression, ulong taskId, Type taskDataType)
        {
            var tupleArrayType = TarantoolContext.Instance.GetTarantoolTupleArrayType(TarantoolContext.Instance.GetTarantoolTupleType(_taskIdType, _tubeTaskState, taskDataType));
            var responseData = _box.Call(callExpression, TarantoolTuple.Create(taskId), tupleArrayType);
            if (responseData != null && responseData.Data[0] is TarantoolTuple tarantoolTuple)
            {
                return TubeTask.GetTubeTask(tarantoolTuple);
            }

            return default;
        }
    }
}
