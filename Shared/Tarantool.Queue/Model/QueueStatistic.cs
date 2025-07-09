// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool.Queue"/> tubes statistics.
    /// </summary>
    public class QueueStatistic
    {
        /// <summary>
        /// Gets a <see cref="Hashtable"/> tubes for statistics. Key is <see langword="string"/>, value is <see cref="QueueTubeStatistic"/>.
        /// </summary>
        public Hashtable QueueTubesStatistic { get; } = new Hashtable();
    }
}
