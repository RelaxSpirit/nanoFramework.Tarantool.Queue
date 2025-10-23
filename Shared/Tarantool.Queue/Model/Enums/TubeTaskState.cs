// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.Tarantool.Queue.Model.Enums
{
    /// <summary>
    /// The task_state field takes one of the following values (different queue types support 
    /// different sets of task_state values, so this is a superset).
    /// </summary>
    public enum TubeTaskState : ushort
    {
        /// <summary>
        /// Empty, no task state
        /// </summary>
        _ = 0,

        /// <summary>
        /// The task is ready for execution (the first consumer executing a take request will get it).
        /// </summary>
        READY = 'r',

        /// <summary>
        /// The task has been taken by a consumer.
        /// </summary>
        TAKEN = 't',

        /// <summary>
        /// The task has been executed (done) (a task is removed from the queue after it has been executed, so this value will rarely be seen).
        /// </summary>
        DONE = '-',

        /// <summary>
        /// The task is buried (disabled temporarily until further changes).
        /// </summary>
        BURIED = '!',

        /// <summary>
        /// The task is delayed for some time.
        /// </summary>
        DELAYED = '~',
    }
}
