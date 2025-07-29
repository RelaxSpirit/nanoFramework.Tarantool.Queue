// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube statistic. 
    /// Statistics are temporary, they are reset whenever the <see cref="Tarantool"/> server restarts.
    /// </summary>
    public partial class QueueTubeStatistic
    {
#nullable enable
        /// <summary>
        /// Gets <see cref="Tasks"/> statistic info.
        /// </summary>
        public Tasks? TasksInfo { get; private set; } = null;

        /// <summary>
        /// Gets <see cref="Calls"/> statistic info.
        /// </summary>
        public Calls? CallsInfo { get; private set; } = null;

        /// <summary>
        /// Show the the number of requests broken down by the type of request.
        /// </summary>
        public partial class Calls
        {
            /// <summary>
            /// Gets numeric - time allotted to the worker to work on a task, in seconds.
            /// </summary>
            public ulong Ttr { get; private set; }

            /// <summary>
            /// Gets count touched tube tasks.
            /// </summary>
            public ulong Touch { get; private set; }

            /// <summary>
            /// Gets count burred tube tasks.
            /// </summary>
            public ulong Bury { get; private set; }

            /// <summary>
            /// Gets count putted tube tasks.
            /// </summary>
            public ulong Put { get; private set; }

            /// <summary>
            /// Gets count acks tube tasks.
            /// </summary>
            public ulong Ack { get; private set; }

            /// <summary>
            /// Gets count delayed tube tasks.
            /// </summary>
            public ulong Delay { get; private set; }

            /// <summary>
            /// Gets count takes tube tasks.
            /// </summary>
            public ulong Take { get; private set; }

            /// <summary>
            /// Gets count kiks tube tasks.
            /// </summary>
            public ulong Kick { get; private set; }

            /// <summary>
            /// Gets count released tube tasks.
            /// </summary>
            public ulong Release { get; private set; }

            /// <summary>
            /// Gets numeric - time to live for a task put into the queue, in seconds.
            /// </summary>
            public ulong Ttl { get; private set; }

            /// <summary>
            /// Gets count deleted tube tasks.
            /// </summary>
            public ulong Delete { get; private set; }
        }

        /// <summary>
        /// Show the number of tasks in a queue broken down by <see cref="Enums.TubeTaskState"/>.
        /// </summary>
        public partial class Tasks
        {
            /// <summary>
            /// Gets count tasks in state <see cref="Enums.TubeTaskState.TAKEN"/>.
            /// </summary>
            public ulong Taken { get; private set; }

            /// <summary>
            /// Gets count tasks in state <see cref="Enums.TubeTaskState.DONE"/>.
            /// </summary>
            public ulong Done { get; private set; }

            /// <summary>
            /// Gets count tasks in state <see cref="Enums.TubeTaskState.READY"/>.
            /// </summary>
            public ulong Ready { get; private set; }

            /// <summary>
            /// Gets count total tasks.
            /// </summary>
            public ulong Total { get; private set; }

            /// <summary>
            /// Gets count tasks in state <see cref="Enums.TubeTaskState.DELAYED"/>.
            /// </summary>
            public ulong Delayed { get; private set; }

            /// <summary>
            /// Gets count tasks in state <see cref="Enums.TubeTaskState.BURIED"/>.
            /// </summary>
            public ulong Buried { get; private set; }
        }
    }
}
