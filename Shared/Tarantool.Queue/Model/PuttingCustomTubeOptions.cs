// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// Putting custom tube options class.
    /// </summary>
    public class PuttingCustomTubeOptions : TubeOptions
    {
        /// <summary>
        /// Gets or sets <see cref="Tarantool"/>.<see cref="Queue"/> tube type.
        /// </summary>
        public override QueueType QueueType { get; protected set; } = QueueType.CustomTube;

        /// <summary>
        /// Validate putting task option name.
        /// </summary>
        /// <param name="optionName">Option name.</param>
        protected override void ValidateOptionName(string optionName)
        { 
        }
    }
}
