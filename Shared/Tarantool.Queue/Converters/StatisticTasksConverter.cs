// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube statistic.
    /// </summary>
    public partial class QueueTubeStatistic
    {
        /// <summary>
        /// Show the number of tasks in a queue broken down by <see cref="Enums.TubeTaskState"/>.
        /// </summary>
        public partial class Tasks
        {
#nullable enable
            internal class StatisticTasksConverter : IConverter
            {
                public object? Read([NotNull] IMessagePackReader reader)
                {
                    var mapLength = reader.ReadMapLength();
                    if (mapLength == uint.MaxValue)
                    {
                        return null;
                    }

                    var stringConverter = ConverterContext.GetConverter(typeof(string));
                    var ulongConverter = ConverterContext.GetConverter(typeof(ulong));

                    Tasks tasks = new Tasks();

                    for (var i = 0; i < mapLength; i++)
                    {
                        var counterName = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());

                        switch (counterName)
                        {
                            case "taken":
                                tasks.Taken = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "done":
                                tasks.Done = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "ready":
                                tasks.Ready = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "total":
                                tasks.Total = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "delayed":
                                tasks.Delayed = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "buried":
                                tasks.Buried = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            default:
                                reader.SkipToken();
                                break;
                        }
                    }

                    return tasks;
                }

                public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}
