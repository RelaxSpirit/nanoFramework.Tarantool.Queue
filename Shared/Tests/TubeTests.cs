// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics;
using nanoFramework.Tarantool.Queue.Client.Interfaces;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
#if NANOFRAMEWORK_1_0
using nanoFramework.TestFramework;
#endif

namespace nanoFramework.Tarantool.Queue.Tests
{
    /// <summary>
    /// <see cref="Queue"/> tube tests class.
    /// </summary>
    [TestClass]
    public sealed class TubeTests
    {
        /// <summary>
        /// Put test.
        /// </summary>
        [TestMethod]
        public void PutTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("put_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube); 
                
                var testMessage = new TestMessage();

                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    var puttingMessage = tubeTask.TaskData as TestMessage;
                    Assert.IsNotNull(puttingMessage);
                    Assert.AreEqual(testMessage.MessageGuid, puttingMessage.MessageGuid);
                    Assert.AreEqual(testMessage.MessageData, puttingMessage.MessageData);
                    Assert.AreEqual(testMessage.SendDateTime, puttingMessage.SendDateTime);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }

                tube = queue.CreateTube("put_test_fifottl_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.FifoTtl));
                Assert.IsNotNull(tube);

                try
                {
                    var tubeTask = tube.Put(testMessage, new PuttingFiFoTtlTubeOptions() { Priority = 1, Delay = TimeSpan.FromSeconds(1), Ttl = TimeSpan.FromSeconds(10), Ttr = TimeSpan.FromSeconds(11) });
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DELAYED, tubeTask.TaskState);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Take test.
        /// </summary>
        [TestMethod]
        public void TakeTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("take_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    tube.Put(testMessage);

                    var tubeTask = tube.Take(typeof(TestMessage), TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);

                    tube.Ack(typeof(TestMessage), tubeTask.TaskId);

                    var puttingMessage = tubeTask.TaskData as TestMessage;
                    Assert.IsNotNull(puttingMessage);
                    Assert.AreEqual(testMessage.MessageGuid, puttingMessage.MessageGuid);
                    Assert.AreEqual(testMessage.MessageData, puttingMessage.MessageData);
                    Assert.AreEqual(testMessage.SendDateTime, puttingMessage.SendDateTime);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Acs test.
        /// </summary>
        [TestMethod]
        public void AckTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("ack_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    tube.Put(testMessage);

                    var taskDataType = typeof(TestMessage);

                    var takeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(takeTask);

                    var tubeTask = tube.Ack(taskDataType, takeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DONE, tubeTask.TaskState);

                    var puttingMessage = tubeTask.TaskData as TestMessage;
                    Assert.IsNotNull(puttingMessage);
                    Assert.AreEqual(testMessage.MessageGuid, puttingMessage.MessageGuid);
                    Assert.AreEqual(testMessage.MessageData, puttingMessage.MessageData);
                    Assert.AreEqual(testMessage.SendDateTime, puttingMessage.SendDateTime); 
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Bury test.
        /// </summary>
        [TestMethod]
        public void BuryTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("bury_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    var taskDataType = typeof(TestMessage);

                    tubeTask = tube.Bury(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.BURIED, tubeTask.TaskState);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Delete test.
        /// </summary>
        [TestMethod]
        public void DeleteTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("delete_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    var taskDataType = typeof(TestMessage);

                    tubeTask = tube.Delete(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DONE, tubeTask.TaskState);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Release test.
        /// </summary>
        [TestMethod]
        public void ReleaseTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var creationOpton = TubeCreationOptions.GetTubeCreationOptions(QueueType.LimFifoTtl);
                creationOpton.Capacity = 1;

                var tube = queue.CreateTube("release_test_limfifottl_tube", creationOpton);
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var putOption = new PuttingLimFiFoTtlTubeOptions();
                    putOption.Delay = TimeSpan.FromSeconds(1);
                    var taskDataType = typeof(TestMessage);

                    var takeTime = Stopwatch.StartNew();

                    var tubeTask = tube.Put(testMessage, putOption);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DELAYED, tubeTask.TaskState);

                    tubeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(2));
                    takeTime.Stop();

                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);
                    Assert.IsTrue(takeTime.Elapsed.TotalSeconds > 1);

                    takeTime.Reset();
                    tubeTask = tube.Release(taskDataType, tubeTask.TaskId, new PuttingLimFiFoTtlTubeOptions());
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    tubeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(1));

                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);
                    takeTime.Reset();

                    putOption.Delay = TimeSpan.FromSeconds(2);
                    takeTime.Start();

                    tubeTask = tube.Release(taskDataType, tubeTask.TaskId, putOption);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DELAYED, tubeTask.TaskState);
                   
                    tubeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(3));
                    takeTime.Stop();

                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);
                    Assert.IsTrue(takeTime.Elapsed.TotalSeconds > 2);

                    tube.Ack(taskDataType, tubeTask.TaskId);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Peek test.
        /// </summary>
        [TestMethod]
        public void PeekTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("peek_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    var taskDataType = typeof(TestMessage);

                    tubeTask = tube.Peek(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    tubeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);

                    tubeTask = tube.Peek(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);

                    tubeTask = tube.Ack(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DONE, tubeTask.TaskState);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Keck test.
        /// </summary>
        [TestMethod]
        public void KickTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("kick_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    var taskDataType = typeof(TestMessage);

                    tubeTask = tube.Bury(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.BURIED, tubeTask.TaskState);

                    var kickCount = tube.Kick(1);
                    Assert.AreEqual(1u, kickCount);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Touch test.
        /// </summary>
        [TestMethod]
        public void TouchTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("touch_test_utubettl_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.UtubeTtl));
                Assert.IsNotNull(tube);
                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage, new PuttingUtubeTtlTubeOptions("test_utube") { Delay = TimeSpan.FromSeconds(1), Ttl = TimeSpan.FromSeconds(10), Ttr = TimeSpan.FromSeconds(11) });
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.DELAYED, tubeTask.TaskState);

                    tubeTask = tube.Take(testMessage.GetType(), TimeSpan.FromSeconds(2));
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);

                    tubeTask = tube.Touch(testMessage.GetType(), tubeTask.TaskId, TimeSpan.FromSeconds(10));
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);

                    tube.Ack(testMessage.GetType(), tubeTask.TaskId);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Release all test.
        /// </summary>
        [TestMethod]
        public void ReleaseAllTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("releaseall_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);

                    var taskDataType = typeof(TestMessage);

                    tubeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.TAKEN, tubeTask.TaskState);

                    tube.ReleaseAll();

                    tubeTask = tube.Peek(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);
                    Assert.AreEqual(TubeTaskState.READY, tubeTask.TaskState);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }

        /// <summary>
        /// Gets statistics test.
        /// </summary>
        [TestMethod]
        public void GetStatisticsTest()
        {
            using (IAdminQueue queue = TarantoolQueueContext.Instance.GetAdminQueue(TestHelper.GetClientOptions(false, false, userData: "testuser:test_password")))
            {
                var tube = queue.CreateTube("statistics_test_fifo_tube", TubeCreationOptions.GetTubeCreationOptions(QueueType.Fifo));
                Assert.IsNotNull(tube);

                var testMessage = new TestMessage();
                try
                {
                    var tubeTask = tube.Put(testMessage);
                    Assert.IsNotNull(tubeTask);

                    var taskDataType = typeof(TestMessage);

                    tubeTask = tube.Take(taskDataType, TimeSpan.FromSeconds(1));
                    Assert.IsNotNull(tubeTask);

                    tubeTask = tube.Ack(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);

                    tubeTask = tube.Put(new TestMessage());
                    Assert.IsNotNull(tubeTask);

                    tubeTask = tube.Delete(taskDataType, tubeTask.TaskId);
                    Assert.IsNotNull(tubeTask);

                    var statistics = tube.GetStatistics();
                    Assert.IsNotNull(statistics);
                    Assert.IsNotNull(statistics.CallsInfo);
                    Assert.IsNotNull(statistics.TasksInfo);

                    Assert.AreEqual(2u, statistics.CallsInfo.Put);
                    Assert.AreEqual(1u, statistics.CallsInfo.Take);
                    Assert.AreEqual(1u, statistics.CallsInfo.Delete);
                    Assert.AreEqual(2u, statistics.TasksInfo.Done);
                }
                finally
                {
                    queue.DeleteTube(tube.Name);
                }
            }
        }
    }
}
