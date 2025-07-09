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
    /// <see cref="Tarantool.Queue"/> tube statistic.
    /// </summary>
    public partial class QueueTubeStatistic
    {
        /// <summary>
        /// Show the number of tasks in a queue broken down by <see cref="Enums.TubeTaskState"/>.
        /// </summary>
        public partial class Calls
        {
#nullable enable
            internal class StatisticCallsConverter : IConverter
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

                    Calls calls = new Calls();

                    for (var i = 0; i < mapLength; i++)
                    {
                        var counterName = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());

                        switch (counterName)
                        {
                            case "ttr":
                                calls.Ttr = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "ttl":
                                calls.Ttl = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "put":
                                calls.Put = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "take":
                                calls.Take = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "ask":
                                calls.Ack = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "kick":
                                calls.Kick = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "bury":
                                calls.Bury = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "delay":
                                calls.Delay = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "delete":
                                calls.Delete = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "release":
                                calls.Release = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            case "touch":
                                calls.Touch = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                                break;
                            default:
                                reader.SkipToken();
                                break;
                        }
                    }

                    return calls;
                }

                public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
                {
                    throw new System.NotImplementedException();
                }
            }
        }
    }
}
