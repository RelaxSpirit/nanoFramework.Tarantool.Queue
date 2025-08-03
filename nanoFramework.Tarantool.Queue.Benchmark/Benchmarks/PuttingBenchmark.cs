// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Benchmark;
using nanoFramework.Benchmark.Attributes;

namespace nanoFramework.Tarantool.Queue.Benchmark.Benchmarks
{
    /// <summary>
    /// Putting benchmark class.
    /// </summary>
    [IterationCount(50)]
    public class PuttingBenchmark
    {
        /// <summary>
        /// Put benchmark method.
        /// </summary>
        [Benchmark]
        public void PutTasksBenchmark()
        {
            BenchmarkContext.Instance.Put();
        }
    }
}
