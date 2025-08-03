// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.Tarantool.Queue.Client.Interfaces;
using nanoFramework.Tarantool.Queue.Converters;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> context.
    /// </summary>
    public class TarantoolQueueContext
    {
        private static readonly object LockInstance = new object();

#nullable enable
        private static TarantoolQueueContext? _instance = null;
#nullable disable

        /// <summary>
        /// Prevents a default instance of the <see cref="TarantoolQueueContext" /> class from being created.
        /// </summary>
        private TarantoolQueueContext()
        {
            ConverterContext.Add(typeof(QueueState), new QueueStateConverter());
            ConverterContext.Add(typeof(TubeTaskState), new TubeTaskStateConverter());
            ConverterContext.Add(typeof(TubeCreationOptions), new TubeCreationOptionsConverter());
            ConverterContext.Add(typeof(QueueType), new QueueTypeConverter());

            var queueStatisticConverter = new QueueStatisticConverter();
            ConverterContext.Add(typeof(QueueStatistic), queueStatisticConverter);
            ConverterContext.Add(typeof(QueueTubeStatistic), queueStatisticConverter.QueueTubeStatisticConverter);

            ConverterContext.Add(typeof(TubeInfo), new TubeInfo.TubeInfoConverter());
            ConverterContext.Add(typeof(TubeInfo[]), new SimpleArrayConverter(typeof(TubeInfo)));

            ConverterContext.Add(typeof(QueueStatistic[]), new SimpleArrayConverter(typeof(QueueStatistic)));

            var putTubeOptionsConverter = new PutTubeOptionsConverter();
            ConverterContext.Add(typeof(PuttingFiFoTtlTubeOptions), putTubeOptionsConverter);
            ConverterContext.Add(typeof(PuttingLimFiFoTtlTubeOptions), putTubeOptionsConverter);
            ConverterContext.Add(typeof(PuttingUtubeTubeOptions), putTubeOptionsConverter);
            ConverterContext.Add(typeof(PuttingUtubeTtlTubeOptions), putTubeOptionsConverter);
            ConverterContext.Add(typeof(PuttingCustomTubeOptions), putTubeOptionsConverter);

            ConverterContext.Add(typeof(QueueSessionIdentify), new QueueSessionIdentifyConverter());
        }

        /// <summary>
        /// Gets <see cref="TarantoolQueueContext"/> instance.
        /// </summary>
        public static TarantoolQueueContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockInstance)
                    {
                        if (_instance == null)
                        {
                            _instance = new TarantoolQueueContext();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Gets new instance <see cref="Tarantool"/>.<see cref="Queue"/> <see cref="IQueue"/> interface.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns><see cref="IQueue"/> interface.</returns>
        public IQueue GetQueue(string connectionString)
        {
            var clientOptions = new QueueClientOptions(connectionString);

            return GetQueue(clientOptions);
        }

        /// <summary>
        /// Gets new instance <see cref="Tarantool"/>.<see cref="Queue"/> <see cref="IAdminQueue"/> interface.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns><see cref="IAdminQueue"/> interface.</returns>
        public IAdminQueue GetAdminQueue(string connectionString)
        {
            var clientOptions = new QueueClientOptions(connectionString);

            return GetAdminQueue(clientOptions);
        }

        /// <summary>
        /// Gets new instance <see cref="Tarantool"/>.<see cref="Queue"/> <see cref="IQueue"/> interface.
        /// </summary>
        /// <param name="clientOptions">Client options.</param>
        /// <returns><see cref="IQueue"/> interface.</returns>
        public IQueue GetQueue(QueueClientOptions clientOptions)
        {
            return new Client.Queue(clientOptions);
        }

        /// <summary>
        /// Gets new instance <see cref="Tarantool"/>.<see cref="Queue"/> <see cref="IAdminQueue"/> interface.
        /// </summary>
        /// <param name="clientOptions">Client options.</param>
        /// <returns><see cref="IAdminQueue"/> interface.</returns>
        public IAdminQueue GetAdminQueue(QueueClientOptions clientOptions)
        {
            return new Client.Queue(clientOptions);
        }
    }
}
