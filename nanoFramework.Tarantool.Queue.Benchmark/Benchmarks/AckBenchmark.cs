// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;

namespace nanoFramework.Tarantool.Queue.Benchmark.Benchmarks
{
    /// <summary>
    /// Ack benchmark class.
    /// </summary>
    [IterationCount(50)]
    public class AckBenchmark
    {
        private ulong _taskid = 0;

        /// <summary>
        /// Ack benchmark method.
        /// </summary>
        [Benchmark]
        public void AckTasksBenchmark()
        {
            BenchmarkContext.Instance.Ack(_taskid++);
        }
    }
}
