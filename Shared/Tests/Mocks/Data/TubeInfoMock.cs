// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Queue.Model;

namespace nanoFramework.Tarantool.Tests.Mocks.Data
{ 
    internal class TubeInfoMock
    {
#nullable enable
        /// <summary>
        /// Gets or sets  <see cref="Tarantool"/>.<see cref="Queue"/> tube id.
        /// </summary>
        internal int TubeId { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Tarantool"/>.<see cref="Queue"/> tube name.
        /// </summary>
        internal string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets queue tube creation options.
        /// </summary>
        internal TubeCreationOptions? CreationOptions { get; set; }
    }
}
