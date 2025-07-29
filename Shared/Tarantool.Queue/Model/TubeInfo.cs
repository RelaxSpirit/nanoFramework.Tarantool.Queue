// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube info.
    /// </summary>
    public partial class TubeInfo
    {
#nullable enable
        /// <summary>
        /// Gets <see cref="Tarantool"/>.<see cref="Queue"/> tube id.
        /// </summary>
        public int TubeId { get; private set; }

        /// <summary>
        /// Gets <see cref="Tarantool"/>.<see cref="Queue"/> tube name.
        /// </summary>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets queue tube creation options.
        /// </summary>
        public TubeCreationOptions? CreationOptions { get; private set; }
    }
}
