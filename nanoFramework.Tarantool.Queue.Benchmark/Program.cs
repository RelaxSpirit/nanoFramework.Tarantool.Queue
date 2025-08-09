// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Benchmark;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using nanoFramework.Tarantool.Queue.Benchmark.Benchmarks;
using nanoFramework.Tarantool.Queue.Client.Interfaces;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Benchmark
{
    /// <summary>
    /// Main benchmark class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main benchmark method.
        /// </summary>
        public static void Main()
        {
            const string Ssid = "YourSSID";
            const string Password = "YourWifiPassword";

            Console.WriteLine($"Run to target: {SystemInfo.TargetName}");

            CancellationTokenSource cs = new CancellationTokenSource(60000);

            var success = WifiNetworkHelper.ConnectDhcp(Ssid, Password, requiresDateTime: true, token: cs.Token);
            if (!success)
            {
                Console.WriteLine($"Can't connect to the network, error: {WifiNetworkHelper.Status}");
                if (WifiNetworkHelper.HelperException != null)
                {
                    Console.WriteLine($"ex: {WifiNetworkHelper.HelperException}");
                }
            }
            else
            {
                Console.WriteLine("********** Starting benchmarks **********");
                QueueClientOptions clientOptions = new QueueClientOptions($"testuser:test_password@{BenchmarkContext.TarantoolHostIp}:3301");
                clientOptions.ConnectionOptions.WriteStreamBufferSize = 512;
                clientOptions.ConnectionOptions.ReadStreamBufferSize = 512;
                clientOptions.ConnectionOptions.ReadBoxInfoOnConnect = false;
                clientOptions.ConnectionOptions.ReadSchemaOnConnect = false;

                using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(clientOptions))
                {
                    var tube = queue.CreateTube(BenchmarkContext.TubeName, TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                }

                try
                {
                    //// Warming up
                    var stopwatch = Stopwatch.StartNew();
                    BenchmarkContext.Instance.Put();
                    BenchmarkContext.Instance.Take();
                    BenchmarkContext.Instance.Ack(0);
                    stopwatch.Stop();
                    Console.WriteLine($"Warming up time {stopwatch.ElapsedMilliseconds} ms.");
                    ////

                    BenchmarkRunner.RunClass(typeof(PuttingBenchmark));
                    BenchmarkRunner.RunClass(typeof(TakeBenchmark));
                    BenchmarkRunner.RunClass(typeof(AckBenchmark));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error start benchmarks\n{ex}");
                }
                finally
                {
                    BenchmarkContext.Instance.CloseConnection();

                    using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(clientOptions))
                    {
                        queue[BenchmarkContext.TubeName].ReleaseAll();

                        queue.DeleteTube(BenchmarkContext.TubeName);
                    }
                }

                Console.WriteLine("********** Completed benchmarks **********");
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
