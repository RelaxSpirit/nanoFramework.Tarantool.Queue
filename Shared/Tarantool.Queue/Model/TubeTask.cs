// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using nanoFramework.Tarantool.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube task.
    /// </summary>
    public struct TubeTask
    {
#nullable enable
        /// <summary>
        /// Gets new <see cref="TubeTask"/> instance.
        /// </summary>
        /// <param name="tarantoolTuple"><see cref="Tarantool"/> tuple values.</param>
        /// <returns>New <see cref="TubeTask"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><see cref="Tarantool"/> tuple parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="Tarantool"/> tuple parameter contains less 3 item values.</exception>
        /// <exception cref="NotSupportedException"><see cref="Tarantool"/> tuple parameter not contains task id or <see cref="TubeTaskState"/> value.</exception>
        public static TubeTask GetTubeTask(TarantoolTuple tarantoolTuple)
        {
            if (tarantoolTuple == null)
            {
                throw new ArgumentNullException();
            }

            if (tarantoolTuple.Length < 3)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (tarantoolTuple[0] is ulong taskId && tarantoolTuple[1] is TubeTaskState tubeTaskState)
            {
                return new TubeTask()
                {
                    TaskId = taskId,
                    TaskState = tubeTaskState,
                    TaskData = tarantoolTuple[2]
                };
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets tube task id.
        /// </summary>
        public ulong TaskId { get; private set; }

        /// <summary>
        /// Gets tube task <see cref="Enums.TubeTaskState"/>.
        /// </summary>
        public TubeTaskState TaskState { get; private set; }

        /// <summary>
        /// Gets tube task data.
        /// </summary>
        public object? TaskData { get; private set; }
    }
}
