// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Client.Interfaces
{
    /// <summary>
    /// Administrate <see cref="Tarantool"/>.<see cref="Queue"/> interface.
    /// </summary>
    public interface IAdminQueue : IQueue
    {
#nullable enable
        /// <summary>
        /// Creating a new queue. Effect: a tuple is added in the _queue space, and a new associated space is created.
        /// </summary>
        /// <param name="tubeName">The queue name must be alphanumeric and be up to 32 characters long.</param>
        /// <param name="options">Creation queue options. The queue options by queue type must be <see cref="QueueType.Fifo"/> or <see cref="QueueType.FifoTtl"/> or <see cref="QueueType.LimFifoTtl"/> or <see cref="QueueType.Utube"/>, or <see cref="QueueType.UtubeTtl"/> or <see cref="QueueType.CustomTube"/>.</param>
        /// <returns>Interface <see cref="ITube"/> a new instance <see cref="Tarantool"/>.<see cref="Queue"/> tube.</returns>
        ITube? CreateTube(string tubeName, TubeCreationOptions options);

        /// <summary>
        /// Dropping a queue. Reverse the effect of a <see cref="CreateTube(string, TubeCreationOptions)"/> request.
        /// Effect: remove the tuple from the _queue space, and drop the space associated with the queue.
        /// </summary>
        /// <param name="tubeName"><see cref="Tarantool"/>.<see cref="Queue"/> tube name.</param>
        void DeleteTube(string tubeName);
    }
}
