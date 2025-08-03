// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Tarantool.Queue.Client.Interfaces;

namespace nanoFramework.Tarantool.Queue.Benchmark
{
    internal class BenchmarkContext
    {
        internal const string TarantoolHostIp = "192.168.1.116";
        internal const string TubeName = "benchmark_fifo_tube";
#nullable enable
        private static object _lock = new object();
        private static BenchmarkContext? _instance = null;

        private readonly IQueue _queue;
        private readonly ITube _tube;

        private readonly string _testMessage = "Hallo nanoFramework Tarantool Queue benchmark!";
        private readonly Type _testMessageType = typeof(string);

        private readonly TimeSpan _takeTimeout = TimeSpan.FromSeconds(1);

        private BenchmarkContext()
        {
            QueueClientOptions clientOptions = new QueueClientOptions($"{TarantoolHostIp}:3301");
            clientOptions.ConnectionOptions.WriteStreamBufferSize = 512;
            clientOptions.ConnectionOptions.ReadStreamBufferSize = 512;
            clientOptions.ConnectionOptions.ReadBoxInfoOnConnect = false;
            clientOptions.ConnectionOptions.ReadSchemaOnConnect = false;
            _queue = TarantoolQueueContext.Instance.GetQueue(clientOptions);

            _tube = _queue[TubeName];
        }

        internal static BenchmarkContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BenchmarkContext();
                        }
                    }
                }

                return _instance;
            }
        }

        internal void Put()
        {
            _tube.Put(_testMessage);
        }

        internal void Take()
        {
            _tube.Take(_testMessageType, _takeTimeout);
        }

        internal void Ack(ulong taskId)
        {
            _tube.Ack(_testMessageType, taskId);
        }
    }
}
