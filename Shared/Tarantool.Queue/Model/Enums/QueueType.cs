// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.Tarantool.Queue.Model.Enums
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> types.
    /// </summary>
    public enum QueueType
    {
        /// <summary>
        /// A simple queue.
        /// </summary>
        Fifo,

        /// <summary>
        /// A simple priority queue with support for task time to live.
        /// </summary>
        FifoTtl,

        /// <summary>
        /// A simple size-limited priority queue with support for task time to live.
        /// </summary>
        Utube,

        /// <summary>
        /// A queue with sub-queues inside.
        /// </summary>
        UtubeTtl,

        /// <summary>
        /// Extension of utube to support ttl.
        /// </summary>
        LimFifoTtl,

        /// <summary>
        /// A not standard <see cref="Tarantool.Queue"/>, customize tube created in <see cref="Tarantool.Queue"/> instance.
        /// </summary>
        CustomTube
    }
}
