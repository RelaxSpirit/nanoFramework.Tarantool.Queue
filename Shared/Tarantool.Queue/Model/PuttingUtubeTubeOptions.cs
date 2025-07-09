// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// A queue with sub-queues inside class.
    /// The main idea of this queue backend is the same as in a <see cref="QueueType.Fifo"/> queue:
    /// the tasks are executed in FIFO order. However, tasks may be grouped into sub-queues.
    /// </summary>
    public class PuttingUtubeTubeOptions : TubeOptions
    {
        internal const string UTUBE = "utube";

        /// <summary>
        /// Initializes a new instance of the <see cref="PuttingUtubeTubeOptions"/> class.
        /// </summary>
        /// <param name="uTubeName">Utube name.</param>
        public PuttingUtubeTubeOptions(string uTubeName)
        {
            UtubeName = uTubeName;
        }

        /// <summary>
        /// Gets or sets <see cref="Tarantool.Queue"/> tube type.
        /// </summary>
        public override QueueType QueueType { get; protected set; } = QueueType.Utube;

        /// <summary>
        /// Gets  the name of the sub-queue. Sub-queues split the task stream according to the sub-queue name:
        /// it is not possible to take two tasks out of a sub-queue concurrently, each sub-queue is executed in strict FIFO order, one task at a time.
        /// </summary>
        public string UtubeName
        {
            get
            {
                var utube = this[UTUBE]?.ToString();
                return utube?.Substring(1, utube.Length - 2) ?? string.Empty;
            }

            private set
            {
                this[UTUBE] = $"'{value}'";
            }
        }

        /// <summary>
        /// Validate putting task option name.
        /// </summary>
        /// <param name="optionName">Option name.</param>
        /// <exception cref="System.NotSupportedException">It is always returned for the queue type <see cref="Enums.QueueType.Utube"/>.</exception>
        protected override void ValidateOptionName(string optionName)
        {
            if (optionName != UTUBE)
            {
                throw GetValidateOptionNameException(optionName);
            }
        }
    }
}
