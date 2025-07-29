// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Queue.Converters;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Tests.Mocks.Converters
{
    internal class QueueStateConverterMock : QueueStateConverter
    {
#nullable enable
        public override void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is QueueState queueState)
            {
                var stringConverter = ConverterContext.GetConverter(typeof(string));

                switch (queueState)
                {
                    case QueueState.RUNNING:
                        stringConverter.Write("RUNNING", writer);
                        break;
                    case QueueState.ENDING:
                        stringConverter.Write("ENDING", writer);
                        break;
                    case QueueState.WAITING:
                        stringConverter.Write("WAITING", writer);
                        break;
                    case QueueState.STARTUP:
                        stringConverter.Write("STARTUP", writer);
                        break;
                    default:
                        stringConverter.Write("INIT", writer);
                        break;
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
