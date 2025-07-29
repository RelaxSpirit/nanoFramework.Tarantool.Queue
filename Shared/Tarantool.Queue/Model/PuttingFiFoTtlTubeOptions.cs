// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// The following options can be specified when putting a task in a <see cref="QueueType.FifoTtl"/> queue.
    /// </summary>
    public class PuttingFiFoTtlTubeOptions : TubeOptions
    {
#nullable enable
        internal const string PRIORITY = "pri";
        internal const string TTL = "ttl";
        internal const string TTR = "ttr";
        internal const string DELAY = "delay";

        private static readonly string[] FifoTtlOptions =
        {
            PRIORITY,
            TTL,
            TTR,
            DELAY
        };

        /// <summary>
        /// Gets or sets task priority (0 is the highest priority and is the default).
        /// </summary>
        public int Priority
        {
            get
            {
                if (TryGetValue(PRIORITY, out object? value) && value != null)
                {
                    return (int)value;
                }
                else
                {
                    return int.MinValue;
                }
            }

            set
            {
                if (value != int.MinValue)
                {
                    this[PRIORITY] = value;
                }
                else
                {
                    Remove(PRIORITY);
                }
            }
        }

        /// <summary>
        /// Gets or sets numeric - time to live for a task put into the queue, in seconds. if ttl is not specified, it is set to infinity
        /// (if a task exists in a queue for longer than ttl seconds, it is removed).
        /// </summary>
        public TimeSpan Ttl
        {
            get
            {
                return GetTimeSpanValue(TTL, TimeSpan.MaxValue);
            }

            set
            {
                SetTimeSpanValue(TTL, value);
            }
        }

        /// <summary>
        /// Gets or sets numeric - time allotted to the worker to work on a task, in seconds; if ttr is not specified, it is set to the same as ttl
        /// (if a task is being worked on for more than ttr seconds, its status is changed to 'ready' so another worker may take it).
        /// </summary>
        public TimeSpan Ttr
        {
            get
            {
                return GetTimeSpanValue(TTR, Ttl);
            }

            set
            {
                SetTimeSpanValue(TTR, value);
            }
        }

        /// <summary>
        /// Gets or sets time to wait before starting to execute the task, in seconds.
        /// </summary>
        public TimeSpan Delay
        {
            get
            {
                return GetTimeSpanValue(DELAY, TimeSpan.Zero);
            }

            set
            {
                SetTimeSpanValue(DELAY, value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Tarantool.Queue"/> tube type.
        /// </summary>
        public override QueueType QueueType { get; protected set; } = QueueType.FifoTtl;

        /// <summary>
        /// Validate putting task option name.
        /// </summary>
        /// <param name="optionName">Option name.</param>
        /// <exception cref="System.NotSupportedException">It is always returned for the queue type <see cref="Enums.QueueType.FifoTtl"/>.</exception>
        protected override void ValidateOptionName(string optionName)
        {
            if (!ContainsOptionKey(optionName, FifoTtlOptions))
            {
                throw GetValidateOptionNameException(optionName);
            }
        }
    }
}
