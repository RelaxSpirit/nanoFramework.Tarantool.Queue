// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// This queue type is effectively a combination of <see cref="QueueType.FifoTtl"/> and <see cref="QueueType.Utube"/>.
    /// </summary>
    public class PuttingUtubeTtlTubeOptions : PuttingUtubeTubeOptions
    {
        private static readonly string[] UtubeTtlOptions =
        {
            UTUBE,
            PuttingFiFoTtlTubeOptions.TTL,
            PuttingFiFoTtlTubeOptions.TTR,
            PuttingFiFoTtlTubeOptions.DELAY
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PuttingUtubeTtlTubeOptions"/> class.
        /// </summary>
        /// <param name="uTubeName">Utube name.</param>
        public PuttingUtubeTtlTubeOptions(string uTubeName) : base(uTubeName)
        {
        }

        /// <summary>
        /// Gets or sets numeric - time to live for a task put into the queue, in seconds. if ttl is not specified, it is set to infinity
        /// (if a task exists in a queue for longer than ttl seconds, it is removed).
        /// </summary>
        public TimeSpan Ttl
        {
            get
            {
                return GetTimeSpanValue(PuttingFiFoTtlTubeOptions.TTL);
            }

            set
            {
                SetTimeSpanValue(PuttingFiFoTtlTubeOptions.TTL, value);
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
                return GetTimeSpanValue(PuttingFiFoTtlTubeOptions.TTR);
            }

            set
            {
                SetTimeSpanValue(PuttingFiFoTtlTubeOptions.TTR, value);
            }
        }

        /// <summary>
        /// Gets or sets time to wait before starting to execute the task, in seconds.
        /// </summary>
        public TimeSpan Delay
        {
            get
            {
                return GetTimeSpanValue(PuttingFiFoTtlTubeOptions.DELAY);
            }

            set
            {
                SetTimeSpanValue(PuttingFiFoTtlTubeOptions.DELAY, value);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="Tarantool.Queue"/> tube type.
        /// </summary>
        public override QueueType QueueType { get; protected set; } = QueueType.UtubeTtl;

        /// <summary>
        /// Validate putting task option name.
        /// </summary>
        /// <param name="optionName">Option name.</param>
        /// <exception cref="System.NotSupportedException">It is always returned for the queue type <see cref="Enums.QueueType.UtubeTtl"/>.</exception>
        protected override void ValidateOptionName(string optionName)
        {
            if (!ContainsOptionKey(optionName, UtubeTtlOptions))
            {
                throw GetValidateOptionNameException(optionName);
            }
        }
    }
}
