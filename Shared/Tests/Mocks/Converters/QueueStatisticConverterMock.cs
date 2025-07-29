// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Queue.Converters;
using nanoFramework.Tarantool.Tests.Mocks.Data;

namespace nanoFramework.Tarantool.Tests.Mocks.Converters
{
    internal class QueueStatisticConverterMock : QueueStatisticConverter
    {
        internal QueueTubeStatisticConverterMock QueueTubeStatisticConverterMock { get; } = new QueueTubeStatisticConverterMock();
#nullable enable
        public override void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is QueueStatisticMock statistic)
            {
                writer.WriteMapHeader((uint)statistic.QueueTubesStatistic.Count);

                var stringConverter = ConverterContext.GetConverter(typeof(string));

                foreach (DictionaryEntry dictionaryEntry in statistic.QueueTubesStatistic)
                {
                    stringConverter.Write(dictionaryEntry.Key.ToString(), writer);
                    QueueTubeStatisticConverterMock.Write(dictionaryEntry.Value, writer);
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
