// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using static nanoFramework.Tarantool.Queue.Model.QueueTubeStatistic.Calls;
using static nanoFramework.Tarantool.Queue.Model.QueueTubeStatistic.Tasks;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube statistic.
    /// </summary>
    public partial class QueueTubeStatistic
    {
#nullable enable
        internal class QueueTubeStatisticConverter : IConverter
        {
            private static readonly StatisticTasksConverter StatisticTasksConverter = new StatisticTasksConverter();
            private static readonly StatisticCallsConverter StatisticCallsConverter = new StatisticCallsConverter();

            public object? Read([NotNull] IMessagePackReader reader)
            {
                var mapLength = reader.ReadMapLength();
                if (mapLength == uint.MaxValue)
                {
                    return null;
                }

                var stringConverter = TarantoolQueueContext.Instance.StringConverter;

                QueueTubeStatistic queueTubeStatistic = new QueueTubeStatistic();

                for (var i = 0; i < mapLength; i++)
                {
                    string partName = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());

                    switch (partName)
                    {
                        case "tasks":
                            queueTubeStatistic.TasksInfo = (Tasks)(StatisticTasksConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                            break;
                        case "calls":
                            queueTubeStatistic.CallsInfo = (Calls)(StatisticCallsConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                            break;
                        default:
                            reader.SkipToken();
                            break;
                    }
                }

                return queueTubeStatistic;
            }

            public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
