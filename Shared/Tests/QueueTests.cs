// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Queue.Client.Interfaces;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
#if NANOFRAMEWORK_1_0
using nanoFramework.TestFramework;
#endif

namespace nanoFramework.Tarantool.Queue.Tests
{
    /// <summary>
    /// <see cref="Queue"/> tests class.
    /// </summary>
    [TestClass]
    public sealed class QueueTests
    {
        /// <summary>
        /// Connect test.
        /// </summary>
        [TestMethod]
        public void ConnectTest()
        {
            using (IQueue queue = TarantoolQueueContext.Instance.GetQueue(TestHelper.GetClientOptions(false, false)))
            {
                Assert.AreNotEqual(string.Empty, queue.Version);
                Assert.AreNotEqual(string.Empty, queue.SessionUuid);
                Assert.AreEqual(QueueState.RUNNING, queue.GetState());
                Assert.IsNotNull(queue.GetStatistics());
            }
        }

        /// <summary>
        /// Admin queue test.
        /// </summary>
        [TestMethod]
        public void AdminQueueTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);
                queue.DeleteTube(tube.Name);

                var creationsOptions = TubeCreationOptions.GetTubeCreationOptions(QueueType.FifoTtl);
                creationsOptions["ttl"] = 10;
                creationsOptions["ttr"] = 11;
                creationsOptions["pri"] = 1;
                tube = queue.CreateTube("test_fifottl_tube", creationsOptions);
                Assert.IsNotNull(tube);
                queue.DeleteTube(tube.Name);

                creationsOptions = TubeCreationOptions.GetTubeCreationOptions(QueueType.LimFifoTtl);
                creationsOptions["ttl"] = 10;
                creationsOptions["ttr"] = 11;
                creationsOptions["pri"] = 1;
                creationsOptions.Capacity = 100;
                tube = queue.CreateTube("test_limfifottl_tube", creationsOptions);
                Assert.IsNotNull(tube);
                queue.DeleteTube(tube.Name);

                tube = queue.CreateTube("test_utube_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Utube));
                Assert.IsNotNull(tube);
                queue.DeleteTube(tube.Name);

                creationsOptions = TubeCreationOptions.GetTubeCreationOptions(QueueType.UtubeTtl);
                creationsOptions["ttl"] = 10;
                creationsOptions["ttr"] = 11;
                creationsOptions["pri"] = 1;
                tube = queue.CreateTube("test_utubettl_tube", creationsOptions);
                Assert.IsNotNull(tube);
                queue.DeleteTube(tube.Name);
            }
        }
    }
}
