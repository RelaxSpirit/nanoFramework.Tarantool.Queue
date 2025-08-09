// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Client.Interfaces
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube interface.
    /// </summary>
    public interface ITube
    {
#nullable enable
        /// <summary>
        /// Gets <see cref="Tarantool"/>.<see cref="Queue"/> tube name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets <see cref="Tarantool"/>.<see cref="Queue"/> tube creation options.
        /// </summary>
        TubeCreationOptions CreationOptions { get; }

        /// <summary>
        /// Gets the queue type (<see cref="QueueType.Fifo"/> or <see cref="QueueType.FifoTtl"/> or <see cref="QueueType.LimFifoTtl"/> or <see cref="QueueType.Utube"/> or <see cref="QueueType.UtubeTtl"/> or <see cref="QueueType.CustomTube"/>).
        /// </summary>
        QueueType TubeType { get; }

        /// <summary>
        /// Gets <see cref="Tarantool"/>.<see cref="Queue"/> tube statistics.
        /// </summary>
        /// <returns><see cref="Tarantool"/>.<see cref="Queue"/> <see cref="QueueTubeStatistic"/></returns>
        QueueTubeStatistic GetStatistics();

        /// <summary>
        /// Putting a task in a queue.
        /// </summary>
        /// <param name="data">The <see cref="TubeTask.TaskData"/> contents are the user-defined description of the task.</param>
        /// <param name="opts">The options, if specified, must be one or more of the options described above
        /// (<see cref="PuttingFiFoTtlTubeOptions.Ttl"/> and/or <see cref="PuttingFiFoTtlTubeOptions.Ttr"/> and/or <see cref="PuttingFiFoTtlTubeOptions.Priority"/> and/or <see cref="PuttingFiFoTtlTubeOptions.Delay"/> and/or <see cref="PuttingUtubeTubeOptions.UtubeName"/>, depending on the queue type).
        /// If an option is not specified, the default is what was specified during <see cref="TubeCreationOptions"/>, and if that was not specified, then the default is what was described above for the queue type.
        /// Note: if the <see cref="PuttingFiFoTtlTubeOptions.Delay"/> option is specified, the delay time is added to the <see cref="PuttingFiFoTtlTubeOptions.Ttl"/> time.</param>
        /// <returns>The value <see cref="TubeTask"/> of the new tuple in the queue's associated space.</returns>
        TubeTask Put([NotNull] object data, TubeOptions? opts = null);

        /// <summary>
        /// Putting a task in a queue.
        /// </summary>
        /// <param name="data">The <see cref="TubeTask.TaskData"/> contents are the user-defined description of the task.</param>
        /// <param name="opts">The options, if specified, must be one or more of the options described above
        /// (<see cref="PuttingFiFoTtlTubeOptions.Ttl"/> and/or <see cref="PuttingFiFoTtlTubeOptions.Ttr"/> and/or <see cref="PuttingFiFoTtlTubeOptions.Priority"/> and/or <see cref="PuttingFiFoTtlTubeOptions.Delay"/> and/or <see cref="PuttingUtubeTubeOptions.UtubeName"/>, depending on the queue type).
        /// If an option is not specified, the default is what was specified during <see cref="TubeCreationOptions"/>, and if that was not specified, then the default is what was described above for the queue type.
        /// Note: if the <see cref="PuttingFiFoTtlTubeOptions.Delay"/> option is specified, the delay time is added to the <see cref="PuttingFiFoTtlTubeOptions.Ttl"/> time.</param>
        void PutWithEmptyResponse([NotNull] object data, TubeOptions? opts = null);

        /// <summary>
        /// The take request searches for a task in the queue or sub-queue (that is, a tuple in the queue's associated space) which has task_state = 'r' (ready),
        /// and task_id = a value lower than any other tuple which also has task_state = 'r'.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Take(Type taskDataType);

        /// <summary>
        /// The take request searches for a task in the queue or sub-queue (that is, a tuple in the queue's associated space) which has task_state = 'r' (ready),
        /// and task_id = a value lower than any other tuple which also has task_state = 'r'.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="timeout">Taken task timeout in seconds. If there is no such task, and timeout was specified, then the job waits until a task becomes ready or the timeout expires.</param>
        /// <param name="opts">Takin options. Use optional to custom tube. Default <see langword="null"/>.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Take(Type taskDataType, TimeSpan timeout, TubeOptions? opts = null);

        /// <summary>
        /// Acknowledging the completion of a task.
        /// The worker which has used 'take' to take the task should use 'ack' to signal that the task has been completed. The current task_state of the tuple should be 't' (taken), and the worker issuing the ack request must have the same ID as the worker which issued the take request.
        /// Effect: the value of task_state changes to '-' (acknowledged). Shortly after this, it may be removed from the queue automatically.
        /// If 'take' occurs but is not soon followed by 'ack' -- that is, if ttr(time to run) expires, or if the worker disconnects -- the effect is: task_state is changed from 't' (taken) back to 'r' (ready). This effect is the same as what would happen with a release request.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="taskId">Task id.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Ack(Type taskDataType, ulong taskId);

        /// <summary>
        /// Acknowledging the completion of a task.
        /// The worker which has used 'take' to take the task should use 'ack' to signal that the task has been completed. The current task_state of the tuple should be 't' (taken), and the worker issuing the ack request must have the same ID as the worker which issued the take request.
        /// Effect: the value of task_state changes to '-' (acknowledged). Shortly after this, it may be removed from the queue automatically.
        /// If 'take' occurs but is not soon followed by 'ack' -- that is, if ttr(time to run) expires, or if the worker disconnects -- the effect is: task_state is changed from 't' (taken) back to 'r' (ready). This effect is the same as what would happen with a release request.
        /// </summary>
        /// <param name="taskId">Task id.</param>
        void Ack(ulong taskId);

        /// <summary>
        /// Deleting a task. Delete the task identified by task_id.
        /// Effect: the current state of task_state is not checked. The task is removed from the queue.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="taskId">Task id.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Delete(Type taskDataType, ulong taskId);

        /// <summary>
        /// Deleting a task. Delete the task identified by task_id.
        /// Effect: the current state of task_state is not checked. The task is removed from the queue.
        /// </summary>
        /// <param name="taskId">Task id.</param>
        void Delete(ulong taskId);

        /// <summary>
        /// Releasing a task. Put the task back in the queue. A worker which has used 'take' to take a task, but cannot complete it, may make a release request instead of an ack request. Effectively, 'ack' implies successful completion of a taken task, and 'release' implies unsuccessful completion of a taken task.
        /// Effect: the value of task_state changes to 'r' (ready). After this, another worker may take it.This is an example of a situation where, due to user intervention, a task may not be successfully completed in strict FIFO order.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="taskId">Task id.</param>
        /// <param name="opts">Release options. Default <see langword="null"/>.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Release(Type taskDataType, ulong taskId, TubeOptions opts);

        /// <summary>
        /// Releasing a task. Put the task back in the queue. A worker which has used 'take' to take a task, but cannot complete it, may make a release request instead of an ack request. Effectively, 'ack' implies successful completion of a taken task, and 'release' implies unsuccessful completion of a taken task.
        /// Effect: the value of task_state changes to 'r' (ready). After this, another worker may take it.This is an example of a situation where, due to user intervention, a task may not be successfully completed in strict FIFO order.
        /// </summary>
        /// <param name="taskId">Task id.</param>
        /// <param name="opts">Release options. Default <see langword="null"/>.</param>
        void Release(ulong taskId, TubeOptions opts);

        /// <summary>
        /// Burying a task. If it becomes clear that a task cannot be executed in the current circumstances, you can "bury" the task -- that is, disable it until the circumstances change.
        /// Effect: the value of task_state changes to '!' (buried). Since '!' is not equal to 'r' (ready), the task can no longer be taken.Since '!' is not equal to '-' (complete), the task will not be deleted.The only thing that can affect a buried task is a kick request.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="taskId">Task id.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Bury(Type taskDataType, ulong taskId);

        /// <summary>
        /// Burying a task. If it becomes clear that a task cannot be executed in the current circumstances, you can "bury" the task -- that is, disable it until the circumstances change.
        /// Effect: the value of task_state changes to '!' (buried). Since '!' is not equal to 'r' (ready), the task can no longer be taken.Since '!' is not equal to '-' (complete), the task will not be deleted.The only thing that can affect a buried task is a kick request.
        /// </summary>
        /// <param name="taskId">Task id.</param>
        void Bury(ulong taskId);

        /// <summary>
        /// Kicking a number of tasks. Effect: the value of task_state changes from '!' (buried) to 'r' (ready), for one or more tasks.
        /// </summary>
        /// <param name="count">Kicking a tasks count.</param>
        /// <returns>Number of tasks actually kicked.</returns>
        ulong Kick(ulong count);

        /// <summary>
        /// Peeking at a task. Look at a task without changing its state.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="taskId">Task id.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Peek(Type taskDataType, ulong taskId);

        /// <summary>
        /// Increasing TTR and/or TTL for tasks. Useful if you can't predict in advance time needed to work on task.
        /// Effect: the value of ttr and ttl increased by increment seconds. If queue does not support ttr, error will be thrown. If increment is lower than zero, error will be thrown. If increment is zero or nil effect is noop.
        /// If current ttr of task is 500 years or greater then operation is noop.
        /// </summary>
        /// <param name="taskDataType">Type of <see cref="TubeTask.TaskData"/> by return.</param>
        /// <param name="taskId">Task id.</param>
        /// <param name="delta">Delta increasing.</param>
        /// <returns>New instance <see cref="TubeTask"/> or <see langword="null"/>.</returns>
        TubeTask? Touch(Type taskDataType, ulong taskId, TimeSpan delta);

        /// <summary>
        /// Increasing TTR and/or TTL for tasks. Useful if you can't predict in advance time needed to work on task.
        /// Effect: the value of ttr and ttl increased by increment seconds. If queue does not support ttr, error will be thrown. If increment is lower than zero, error will be thrown. If increment is zero or nil effect is noop.
        /// If current ttr of task is 500 years or greater then operation is noop.
        /// </summary>
        /// <param name="taskId">Task id.</param>
        /// <param name="delta">Delta increasing.</param>
        void Touch(ulong taskId, TimeSpan delta);

        /// <summary>
        /// Releasing all taken tasks. Forcibly returns all taken tasks to a ready state.
        /// </summary>
        void ReleaseAll();
    }
}
