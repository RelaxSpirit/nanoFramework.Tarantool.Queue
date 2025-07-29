// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// The following options can be specified when putting a task in a <see cref="QueueType.LimFifoTtl"/> queue.
    /// </summary>
    public class PuttingLimFiFoTtlTubeOptions : PuttingFiFoTtlTubeOptions
    {
        private const string TIMEOUT = "timeout";

        private static readonly string[] LimFifoTtlOptions = new string[]
        {
            TIMEOUT
        };

        /// <summary>
        /// Gets or sets <see cref="Tarantool.Queue"/> tube type.
        /// </summary>
        public override QueueType QueueType => QueueType.LimFifoTtl;

        /// <summary>
        /// Gets or sets numeric - seconds to wait until queue has free space,
        /// if timeout is not specified or time is up, and queue has no space, method return Nil.
        /// </summary>
        public TimeSpan Timeout
        {
            get
            {
                return GetTimeSpanValue(TIMEOUT, TimeSpan.MaxValue);
            }

            set
            {
                SetTimeSpanValue(TIMEOUT, value);
            }
        }

        /// <summary>
        /// Validate putting task option name.
        /// </summary>
        /// <param name="optionName">Option name.</param>
        /// <exception cref="System.NotSupportedException">It is always returned for the queue type <see cref="Enums.QueueType.LimFifoTtl"/>.</exception>
        protected override void ValidateOptionName(string optionName)
        {
            if (!ContainsOptionKey(optionName, LimFifoTtlOptions))
            {
                base.ValidateOptionName(optionName);
            }
        }
    }
}
