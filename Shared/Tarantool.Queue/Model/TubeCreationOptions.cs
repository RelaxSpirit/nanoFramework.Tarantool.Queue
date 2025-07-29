// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// Queue tube creation option class.
    /// </summary>
    public class TubeCreationOptions : TubeOptions
    {
#nullable enable
        internal const string TemporaryConst = "temporary";
        internal const string IfNotExistsConst = "if_not_exists";
        internal const string OnTaskChangeConst = "on_task_change";
        internal const string CapacityConst = "capacity";
        internal const string StorageModeConst = "storage_mode";
        internal const string TtlConst = "ttl";
        internal const string PriorityConst = "pri";
        internal const string TtrConst = "ttr";

        private static readonly string[] AllTubeCreationOptions = new string[]
        {
            TemporaryConst,
            IfNotExistsConst,
            OnTaskChangeConst
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="TubeCreationOptions"/> class.
        /// </summary>
        /// <param name="queueType">Queue type.</param>
        private TubeCreationOptions(QueueType queueType)
        {
            QueueType = queueType;
        }

        /// <summary>
        /// Gets or sets queue type.
        /// </summary>
        public override QueueType QueueType { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether option to save the queue to disks.
        /// If <see langword="true"/>, the contents do not persist on disk (the queue is in-memory only).
        /// </summary>
        public bool Temporary
        {
            get
            {
                if (TryGetValue(TemporaryConst, out object? value) && value != null)
                {
                    return (bool)value;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this[TemporaryConst] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether option to ignore an error if the queue already exist.
        /// If <see langword="true"/>, no error will be returned if the tube already exists.
        /// </summary>
        public bool IfNotExists
        {
            get
            {
                if (TryGetValue(IfNotExistsConst, out object? value) && value != null)
                {
                    return (bool)value;
                }
                else
                {
                    return false;
                }
            }

            set
            {
                this[IfNotExistsConst] = value;
            }
        }

        /// <summary>
        /// Gets or sets queue size limiter option.
        /// Number - limit size of the queue.
        /// Implements only <see cref="QueueType.LimFifoTtl"/> queue type.
        /// </summary>
        public ulong Capacity
        {
            get
            {
                if (TryGetValue(CapacityConst, out object? value) && value != null)
                {
                    return (ulong)value;
                }
                else
                {
                    return ulong.MinValue;
                }
            }

            set
            {
                if (value != ulong.MinValue)
                {
                    this[CapacityConst] = value;
                }
                else
                {
                    Remove(CapacityConst);
                }
            }
        }

        /// <summary>
        /// Gets or sets string - one of:
        /// "default" - default implementation of utube;
        /// "ready_buffer" - allows processing take requests faster, but by the cost of put operations speed. Right now this option is supported only for memtx engine;
        /// WARNING: this is an experimental storage mode.
        /// Implements only <see cref="QueueType.Utube"/> or <see cref="QueueType.UtubeTtl"/> queue type.
        /// </summary>
        public string? StorageMode 
        {
            get
            {
                if (TryGetValue(CapacityConst, out object? value) && value != null)
                {
                    return (string)value;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this[CapacityConst] = value;
                }
                else
                {
                    Remove(CapacityConst);
                }
            }
        }

        /// <summary>
        /// Gets default queue tube creation options.
        /// </summary>
        /// <param name="queueType">Queue tube type.</param>
        /// <returns>Default creation queue tube options.</returns>
        public static TubeCreationOptions GetTubeCreationOptions(QueueType queueType)
        {
            return new TubeCreationOptions(queueType);
        }

        /// <summary>
        /// Validate queue tube option name method.
        /// </summary>
        /// <param name="optionName">Queue tube option name.</param>
        /// <exception cref="System.NotSupportedException">If option name is not valid.</exception>
        protected override void ValidateOptionName(string optionName)
        {
            if (QueueType == QueueType.CustomTube)
            {
                return;
            }

            if (optionName == CapacityConst && QueueType == QueueType.LimFifoTtl)
            {
                return;
            }

            if (optionName == StorageModeConst && (QueueType == QueueType.Utube || QueueType == QueueType.UtubeTtl))
            {
                return;
            }

            if ((optionName == TtlConst || optionName == TtrConst || optionName == PriorityConst) && (QueueType == QueueType.LimFifoTtl || QueueType == QueueType.FifoTtl || QueueType == QueueType.UtubeTtl))
            {
                return;
            }

            if (ContainsOptionKey(optionName, AllTubeCreationOptions))
            {
                return;
            }
            
            throw GetValidateCreationOptionNameException(optionName);
        }
    }
}
