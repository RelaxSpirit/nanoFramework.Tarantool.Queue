// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.Tarantool.Queue.Model.Enums
{
    /// <summary>
    /// There are five states for queue.
    /// </summary>
    public enum QueueState
    {
        /// <summary>
        /// Initialize state.
        /// </summary>
        INIT = 'i',

        /// <summary>
        /// Startup state.
        /// </summary>
        STARTUP = 's',

        /// <summary>
        /// Running state.
        /// </summary>
        RUNNING = 'r',

        /// <summary>
        /// Ending state.
        /// </summary>
        ENDING = 'e',

        /// <summary>
        /// Waiting state.
        /// </summary>
        WAITING = 'w',
    }
}
