// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Model;

namespace nanoFramework.Tarantool.Queue
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> client options.
    /// </summary>
    public class QueueClientOptions : ClientOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueClientOptions" /> class.
        /// </summary>
        /// <param name="connectionString"><see cref="Tarantool"/> instance connection string.</param>
        /// <param name="require"><see cref="Tarantool"/>.<see cref="Queue"/> require module name. Default value "queue".</param>
        public QueueClientOptions(string connectionString, string require = "queue") : base(connectionString) 
        {
            Require = require;
        }

        /// <summary>
        /// Gets or sets <see cref="Tarantool"/>.<see cref="Queue"/> require module name. Default value "queue".
        /// </summary>
        public string Require { get; set; }
    }
}
