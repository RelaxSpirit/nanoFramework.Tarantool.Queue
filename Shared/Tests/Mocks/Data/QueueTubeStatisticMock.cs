// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.Tarantool.Tests.Mocks.Data
{
    internal partial class QueueTubeStatisticMock
    {
        /// <summary>
        /// Gets <see cref="Tasks"/> statistic info.
        /// </summary>
        internal Tasks TasksInfo { get; private set; } = new Tasks();

        /// <summary>
        /// Gets <see cref="Calls"/> statistic info.
        /// </summary>
        internal Calls CallsInfo { get; private set; } = new Calls();

        /// <summary>
        /// Show the the number of requests broken down by the type of request.
        /// </summary>
        internal partial class Calls
        {
            /// <summary>
            /// Gets or sets numeric - time allotted to the worker to work on a task, in seconds.
            /// </summary>
            internal ulong Ttr { get; set; }

            /// <summary>
            /// Gets or sets count touched tube tasks.
            /// </summary>
            internal ulong Touch { get; set; }

            /// <summary>
            /// Gets or sets count burred tube tasks.
            /// </summary>
            internal ulong Bury { get; set; }

            /// <summary>
            /// Gets or sets count putted tube tasks.
            /// </summary>
            internal ulong Put { get; set; }

            /// <summary>
            /// Gets or sets count acks tube tasks.
            /// </summary>
            internal ulong Ack { get; set; }

            /// <summary>
            /// Gets or sets count delayed tube tasks.
            /// </summary>
            internal ulong Delay { get; set; }

            /// <summary>
            /// Gets or sets count takes tube tasks.
            /// </summary>
            internal ulong Take { get; set; }

            /// <summary>
            /// Gets or sets count kiks tube tasks.
            /// </summary>
            internal ulong Kick { get; set; }

            /// <summary>
            /// Gets or sets count released tube tasks.
            /// </summary>
            internal ulong Release { get; set; }

            /// <summary>
            /// Gets or sets numeric - time to live for a task put into the queue, in seconds.
            /// </summary>
            internal ulong Ttl { get; set; }

            /// <summary>
            /// Gets or sets count deleted tube tasks.
            /// </summary>
            internal ulong Delete { get; set; }
        }

        /// <summary>
        /// Show the number of tasks in a queue broken down by <see cref="Enums.TubeTaskState"/>.
        /// </summary>
        internal partial class Tasks
        {
            /// <summary>
            /// Gets or sets count tasks in state <see cref="Enums.TubeTaskState.TAKEN"/>.
            /// </summary>
            internal ulong Taken { get;  set; }

            /// <summary>
            /// Gets or sets count tasks in state <see cref="Enums.TubeTaskState.DONE"/>.
            /// </summary>
            internal ulong Done { get; set; }

            /// <summary>
            /// Gets or sets count tasks in state <see cref="Enums.TubeTaskState.READY"/>.
            /// </summary>
            internal ulong Ready { get; set; }

            /// <summary>
            /// Gets or sets count total tasks.
            /// </summary>
            internal ulong Total { get; set; }

            /// <summary>
            /// Gets or sets count tasks in state <see cref="Enums.TubeTaskState.DELAYED"/>.
            /// </summary>
            internal ulong Delayed { get; set; }

            /// <summary>
            /// Gets or sets count tasks in state <see cref="Enums.TubeTaskState.BURIED"/>.
            /// </summary>
            internal ulong Buried { get; set; }
        }
    }
}
