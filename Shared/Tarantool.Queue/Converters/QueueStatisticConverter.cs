// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class QueueStatisticConverter : IConverter
    {
#nullable enable
        internal QueueTubeStatistic.QueueTubeStatisticConverter QueueTubeStatisticConverter { get; } = new QueueTubeStatistic.QueueTubeStatisticConverter();

        public object? Read([NotNull] IMessagePackReader reader)
        {
            var mapLength = reader.ReadMapLength();
            if (mapLength == uint.MaxValue)
            {
                return null;
            }

            var stringConverter = ConverterContext.GetConverter(typeof(string));

            QueueStatistic queueStatistic = new QueueStatistic();

            for (var i = 0; i < mapLength; i++)
            {
                queueStatistic.QueueTubesStatistic.Add(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference(), QueueTubeStatisticConverter.Read(reader));
            }

            return queueStatistic;
        }

        public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
