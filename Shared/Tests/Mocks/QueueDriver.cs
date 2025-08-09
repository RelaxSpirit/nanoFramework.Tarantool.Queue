// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using System.Diagnostics.CodeAnalysis;
#if NANOFRAMEWORK_1_0
using System.Threading;
#endif
using nanoFramework.Tarantool.Model;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
using nanoFramework.Tarantool.Queue.Tests;
using nanoFramework.Tarantool.Tests.Mocks.Data;

namespace nanoFramework.Tarantool.Tests.Mocks
{
    internal class QueueDriver
    {
        private readonly object _lockTasks = new object();
        private readonly object _lockStatistics = new object();
        private readonly Hashtable _tasks = new Hashtable();
        private int _taskId = -1;

        internal QueueDriver(int id, TubeCreationOptions creationOptions)
        {
            Id = id;
            CreationOptions = creationOptions;
        }

        internal int Id { get; }

        internal QueueTubeStatisticMock QueueTubeStatistic { get; } = new QueueTubeStatisticMock();

        internal TubeCreationOptions CreationOptions { get; }

        internal TarantoolTuple Take(TimeSpan timeout)
        {
            DateTime dateTime = DateTime.UtcNow;

            while (DateTime.UtcNow - dateTime < timeout)
            {
                lock (_lockTasks)
                {
                    foreach (DictionaryEntry dictionaryEntry in _tasks)
                    {
                        if (dictionaryEntry.Value is ArrayList taskData && taskData[0] is TubeTaskState tubeTaskState && (tubeTaskState == TubeTaskState.READY || tubeTaskState == TubeTaskState.DELAYED))
                        {
                            if (CreationOptions.QueueType != QueueType.Fifo && CreationOptions.QueueType != QueueType.Utube)
                            {
                                if ((tubeTaskState == TubeTaskState.DELAYED && taskData[2] is DateTime canBeTakeTime && canBeTakeTime > DateTime.UtcNow) || (taskData[3] is DateTime deadTime && deadTime <= DateTime.UtcNow))
                                {
                                    continue;
                                }
                            }

                            taskData[0] = TubeTaskState.TAKEN;

                            lock (_lockStatistics)
                            {
                                QueueTubeStatistic.CallsInfo.Take++;
                                if (tubeTaskState == TubeTaskState.DELAYED)
                                {
                                    QueueTubeStatistic.TasksInfo.Delayed--;
                                }
                                else
                                {
                                    QueueTubeStatistic.TasksInfo.Ready--;
                                }

                                QueueTubeStatistic.TasksInfo.Taken++;
                            }

                            return TarantoolTuple.Create((int)dictionaryEntry.Key, taskData[0], taskData[1]);
                        }
                    }
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            
            return TarantoolTuple.Empty;
        }

        internal TarantoolTuple Touch(int taskId, TimeSpan delta)
        {
            lock (_lockTasks)
            {
                if (_tasks[taskId] is ArrayList taskData && taskData.Count > 2)
                {
                    if (taskData[2] is DateTime canBeTakeTime && taskData[3] is DateTime deadTime)
                    {
                        if (canBeTakeTime < DateTime.MaxValue)
                        {
                            taskData[2] = canBeTakeTime += delta;
                        }

                        if (deadTime < DateTime.MaxValue)
                        {
                            taskData[3] = deadTime += delta;
                        }
                    }

                    return TarantoolTuple.Create(taskId, taskData[0], taskData[1]);
                }
            }

            return TarantoolTuple.Empty;
        }

        internal ulong Kick(int count)
        {
            int actualCount = 0;
            lock (_lockTasks)
            {
                foreach (ArrayList taskData in _tasks.Values)
                {
                    if (taskData[0] is TubeTaskState tubeTaskState && tubeTaskState == TubeTaskState.BURIED)
                    {
                        taskData[0] = TubeTaskState.READY;
                        actualCount++;
                        lock (_lockStatistics)
                        {
                            QueueTubeStatistic.CallsInfo.Kick++;
                            QueueTubeStatistic.TasksInfo.Buried--;
                            QueueTubeStatistic.TasksInfo.Ready++;
                        }
                    }

                    if (actualCount >= count)
                    {
                        break;
                    }
                }
            }

            return (ulong)actualCount;
        }

        internal TarantoolTuple Peek(int taskId)
        {
            lock (_lockTasks)
            {
                if (_tasks[taskId] is ArrayList taskData)
                {
                    return TarantoolTuple.Create(taskId, taskData[0], taskData[1]);
                }
            }

            return TarantoolTuple.Empty;
        }
#nullable enable
        internal TarantoolTuple Put([NotNull] object data, Hashtable? opts = null)
        {
            var taskId = Interlocked.Increment(ref _taskId);
            ArrayList taskData = new ArrayList();
            taskData.Add(TubeTaskState.READY);
            if (data is Hashtable hashtable)
            {
                taskData.Add(TestMessage.HashtableToTestMessage(hashtable));
            }
            else
            {
                taskData.Add(data);
            }

            switch (CreationOptions.QueueType)
            {
                case QueueType.FifoTtl:
                case QueueType.LimFifoTtl:
                case QueueType.UtubeTtl:
                    DateTime canBeReadTime = DateTime.UtcNow;
                    DateTime deadTime = DateTime.MaxValue;
                    int pri = 0;
                    TimeSpan workLimitTime = TimeSpan.MaxValue;

                    if (opts != null)
                    {
                        if (CreationOptions.QueueType == QueueType.FifoTtl || CreationOptions.QueueType == QueueType.LimFifoTtl || CreationOptions.QueueType == QueueType.UtubeTtl)
                        {
                            if (opts[PuttingFiFoTtlTubeOptions.DELAY] != null)
                            {
                                canBeReadTime += TimeSpan.FromSeconds(long.Parse(opts[PuttingFiFoTtlTubeOptions.DELAY]?.ToString() ?? throw new NullReferenceException()));
                            }
                            else if (CreationOptions[PuttingFiFoTtlTubeOptions.DELAY] is TimeSpan delay)
                            {
                                canBeReadTime += delay;
                            }

                            if (opts[PuttingFiFoTtlTubeOptions.TTL] != null)
                            {
                                deadTime = DateTime.UtcNow + TimeSpan.FromSeconds(long.Parse(opts[PuttingFiFoTtlTubeOptions.TTL]?.ToString() ?? throw new NullReferenceException()));
                            }
                            else if (CreationOptions[PuttingFiFoTtlTubeOptions.TTL] is TimeSpan ttl)
                            {
                                deadTime = DateTime.UtcNow + ttl;
                            }

                            if (opts[PuttingFiFoTtlTubeOptions.TTR] != null)
                            {
                                workLimitTime = TimeSpan.FromSeconds(long.Parse(opts[PuttingFiFoTtlTubeOptions.TTR]?.ToString() ?? throw new NullReferenceException()));
                            }
                            else if (CreationOptions[PuttingFiFoTtlTubeOptions.TTR] is TimeSpan ttr)
                            {
                                workLimitTime = ttr;
                            }

                            if (opts[PuttingFiFoTtlTubeOptions.PRIORITY] != null)
                            {
                                pri = int.Parse(opts[PuttingFiFoTtlTubeOptions.PRIORITY]?.ToString() ?? throw new NullReferenceException());
                            }
                            else if (CreationOptions[PuttingFiFoTtlTubeOptions.PRIORITY] is int priority)
                            {
                                pri = priority;
                            }
                        }
                    }
                    else
                    {
                        if (CreationOptions.QueueType == QueueType.FifoTtl || CreationOptions.QueueType == QueueType.LimFifoTtl || CreationOptions.QueueType == QueueType.UtubeTtl)
                        {
                            if (CreationOptions[PuttingFiFoTtlTubeOptions.DELAY] is TimeSpan delay)
                            {
                                canBeReadTime += delay;
                            }

                            if (CreationOptions[PuttingFiFoTtlTubeOptions.TTL] is TimeSpan ttl)
                            {
                                deadTime = DateTime.UtcNow + ttl;
                            }

                            if (CreationOptions[PuttingFiFoTtlTubeOptions.TTR] is TimeSpan ttr)
                            {
                                workLimitTime = ttr;
                            }

                            if (CreationOptions[PuttingFiFoTtlTubeOptions.PRIORITY] is int priority)
                            {
                                pri = priority;
                            }
                        }
                    }

                    if (canBeReadTime > DateTime.UtcNow)
                    {
                        taskData[0] = TubeTaskState.DELAYED;
                        QueueTubeStatistic.TasksInfo.Delayed++;
                    }

                    taskData.Add(canBeReadTime);
                    taskData.Add(deadTime);
                    taskData.Add(workLimitTime);
                    taskData.Add(pri);

                    if (CreationOptions.QueueType == QueueType.Utube || CreationOptions.QueueType == QueueType.UtubeTtl)
                    {
                        if (opts != null && opts["utube"] is string utubeName)
                        {
                            taskData.Add(utubeName);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }

                    break;
            }

            lock (_lockTasks)
            {
                _tasks.Add(taskId, taskData);
            }

            lock (_lockStatistics)
            {
                QueueTubeStatistic.CallsInfo.Put++;
                QueueTubeStatistic.TasksInfo.Total++;
            }

            return TarantoolTuple.Create((ulong)taskId, taskData[0], taskData[1]);
        }

        internal TarantoolTuple Release(int taskId, Hashtable? opts)
        {
            lock (_lockTasks)
            {
                if (_tasks[taskId] is ArrayList taskData && taskData[0] is TubeTaskState tubeTaskState && tubeTaskState == TubeTaskState.TAKEN)
                {
                    if (CreationOptions.QueueType == QueueType.Fifo || CreationOptions.QueueType == QueueType.Utube)
                    {
                        taskData[0] = TubeTaskState.READY;
                        
                        lock (_lockStatistics)
                        {
                            QueueTubeStatistic.CallsInfo.Release++;
                            QueueTubeStatistic.TasksInfo.Taken--;
                            QueueTubeStatistic.TasksInfo.Ready++;
                        }
                    }
                    else
                    {
                        DateTime canBeReadTime = DateTime.UtcNow;
                        if (opts != null)
                        {
                            if (opts[PuttingFiFoTtlTubeOptions.DELAY] != null)
                            {
                                canBeReadTime += TimeSpan.FromSeconds(long.Parse(opts[PuttingFiFoTtlTubeOptions.DELAY]?.ToString() ?? throw new NullReferenceException()));
                            }
                            else if (CreationOptions[PuttingFiFoTtlTubeOptions.DELAY] is TimeSpan delay)
                            {
                                canBeReadTime += delay;
                            }
                        }
                        else
                        {
                            if (CreationOptions[PuttingFiFoTtlTubeOptions.DELAY] is TimeSpan delay)
                            {
                                canBeReadTime += delay;
                            }
                        }

                        if (canBeReadTime > DateTime.UtcNow)
                        {
                            taskData[0] = TubeTaskState.DELAYED;
                            taskData[2] = canBeReadTime;
                            lock (_lockStatistics)
                            {
                                QueueTubeStatistic.CallsInfo.Release++;
                                QueueTubeStatistic.TasksInfo.Taken--;
                                QueueTubeStatistic.TasksInfo.Delayed++;
                            }
                        }
                        else
                        {
                            taskData[0] = TubeTaskState.READY;
                            lock (_lockStatistics)
                            {
                                QueueTubeStatistic.CallsInfo.Release++;
                                QueueTubeStatistic.TasksInfo.Taken--;
                                QueueTubeStatistic.TasksInfo.Ready++;
                            }
                        }
                    }

                    var returnValue = TarantoolTuple.Create(taskId, taskData[0], taskData[1]);
                    return returnValue;
                }
            }

            throw new NotSupportedException("Task was not taken");
        }
#nullable disable

        internal void ReleaseAll()
        {
            lock (_lockTasks)
            {
                foreach (ArrayList taskData in _tasks.Values)
                {
                    if (taskData[0] is TubeTaskState tubeTaskState && tubeTaskState == TubeTaskState.TAKEN)
                    {
                        taskData[0] = TubeTaskState.READY;

                        lock (_lockStatistics)
                        {
                            QueueTubeStatistic.TasksInfo.Taken--;
                            QueueTubeStatistic.TasksInfo.Ready++;
                        }
                    }
                }
            }
        }

        internal TarantoolTuple Ack(int taskId)
        {
            lock (_lockTasks)
            {
                if (_tasks[taskId] is ArrayList taskData && taskData[0] is TubeTaskState tubeTaskState && tubeTaskState == TubeTaskState.TAKEN)
                {
                    taskData[0] = TubeTaskState.DONE;
                    var returnValue = TarantoolTuple.Create(taskId, taskData[0], taskData[1]);
                    lock (_lockStatistics)
                    {
                        QueueTubeStatistic.CallsInfo.Ack++;
                        QueueTubeStatistic.TasksInfo.Taken--;
                        QueueTubeStatistic.TasksInfo.Done++;
                    }

                    _tasks.Remove(taskId);
                    return returnValue;
                }
            }

            return TarantoolTuple.Empty;
        }

        internal TarantoolTuple Bury(int taskId)
        {
            lock (_lockTasks)
            {
                if (_tasks[taskId] is ArrayList taskData && taskData[0] is TubeTaskState tubeTaskState)
                {
                    taskData[0] = TubeTaskState.BURIED;
                    var returnValue = TarantoolTuple.Create(taskId, taskData[0], taskData[1]);
                    lock (_lockStatistics)
                    {
                        QueueTubeStatistic.CallsInfo.Bury++;
                        switch (tubeTaskState)
                        {
                            case TubeTaskState.READY:
                                QueueTubeStatistic.TasksInfo.Ready--;
                                break;
                            case TubeTaskState.DELAYED:
                                QueueTubeStatistic.TasksInfo.Delayed--;
                                break;
                            case TubeTaskState.TAKEN:
                                QueueTubeStatistic.TasksInfo.Taken--;
                                break;
                        }
                    }

                    return returnValue;
                }
            }

            throw new NotSupportedException("Task was not taken");
        }

        internal TarantoolTuple Delete(int taskId)
        {
            lock (_lockTasks)
            {
                if (_tasks[taskId] is ArrayList taskData && taskData[0] is TubeTaskState tubeTaskState)
                {
                    taskData[0] = TubeTaskState.DONE;
                    var returnValue = TarantoolTuple.Create(taskId, taskData[0], taskData[1]);

                    lock (_lockStatistics)
                    {
                        QueueTubeStatistic.CallsInfo.Delete++;

                        switch (tubeTaskState)
                        {
                            case TubeTaskState.READY:
                                QueueTubeStatistic.TasksInfo.Ready--;
                                break;
                            case TubeTaskState.DELAYED:
                                QueueTubeStatistic.TasksInfo.Delayed--;
                                break;
                            case TubeTaskState.BURIED:
                                QueueTubeStatistic.TasksInfo.Buried--;
                                break;
                            case TubeTaskState.TAKEN:
                                QueueTubeStatistic.TasksInfo.Taken--;
                                break;
                        }

                        QueueTubeStatistic.TasksInfo.Done++;
                    }

                    _tasks.Remove(taskId);
                    return returnValue;
                }
            }

            return TarantoolTuple.Empty;
        }
    }
}
