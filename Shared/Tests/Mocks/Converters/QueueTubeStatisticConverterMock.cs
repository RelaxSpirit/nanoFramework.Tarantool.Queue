// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Tests.Mocks.Data;

namespace nanoFramework.Tarantool.Tests.Mocks.Converters
{
    internal class QueueTubeStatisticConverterMock : IConverter
    {
#nullable enable
        public object? Read([NotNull] IMessagePackReader reader)
        {
            throw new NotImplementedException();
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is QueueTubeStatisticMock queueTubeStatisticMock)
            {
                writer.WriteMapHeader(2);

                var stringConverter = ConverterContext.GetConverter(typeof(string));
                var ulongConverter = ConverterContext.GetConverter(typeof(ulong));

                stringConverter.Write("tasks", writer);
                writer.WriteMapHeader(6);

                stringConverter.Write("taken", writer);
                ulongConverter.Write(queueTubeStatisticMock.TasksInfo.Taken, writer);

                stringConverter.Write("done", writer);
                ulongConverter.Write(queueTubeStatisticMock.TasksInfo.Done, writer);

                stringConverter.Write("ready", writer);
                ulongConverter.Write(queueTubeStatisticMock.TasksInfo.Ready, writer);

                stringConverter.Write("total", writer);
                ulongConverter.Write(queueTubeStatisticMock.TasksInfo.Total, writer);

                stringConverter.Write("delayed", writer);
                ulongConverter.Write(queueTubeStatisticMock.TasksInfo.Delayed, writer);

                stringConverter.Write("buried", writer);
                ulongConverter.Write(queueTubeStatisticMock.TasksInfo.Buried, writer);

                stringConverter.Write("calls", writer);
                writer.WriteMapHeader(11);

                stringConverter.Write("ttr", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Ttr, writer);

                stringConverter.Write("ttl", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Ttl, writer);

                stringConverter.Write("put", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Put, writer);

                stringConverter.Write("take", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Take, writer);

                stringConverter.Write("ask", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Ack, writer);

                stringConverter.Write("kick", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Kick, writer);

                stringConverter.Write("bury", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Bury, writer);

                stringConverter.Write("delay", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Delay, writer);

                stringConverter.Write("delete", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Delete, writer);

                stringConverter.Write("release", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Release, writer);

                stringConverter.Write("touch", writer);
                ulongConverter.Write(queueTubeStatisticMock.CallsInfo.Touch, writer);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
