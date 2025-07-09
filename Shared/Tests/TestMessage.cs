// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;

#if NANOFRAMEWORK_1_0
using nanoFramework.TestFramework;
#endif

namespace nanoFramework.Tarantool.Queue.Tests
{
    internal class TestMessage
    {
        internal Guid MessageGuid { get; private set; } = Guid.NewGuid();

        internal DateTime SendDateTime { get; private set; } = DateTime.UtcNow;

        internal string MessageData { get; private set; } = "Hello nanoFramework!";

        internal class TestMessageConverter : IConverter
        {
#nullable enable
            public object? Read([NotNull] IMessagePackReader reader)
            {
                var length = reader.ReadMapLength();
                var stringConverter = ConverterContext.GetConverter(typeof(string));
                TestMessage testMessage = new TestMessage();

                for (int i  = 0; i < length; i++)
                {
                    string fieldName = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());

                    switch (fieldName)
                    {
                        case "MessageData":
                            testMessage.MessageData = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                            break;
                        case "SendDateTime":
                            testMessage.SendDateTime = (DateTime)(ConverterContext.GetConverter(typeof(DateTime)).Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                            break;
                        case "MessageGuid":
                            testMessage.MessageGuid = (Guid)(ConverterContext.GetConverter(typeof(Guid)).Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                            break;
                        default:
                            reader.SkipToken();
                            break;
                    }
                }

                return testMessage;
            }

            public void Write(object? value, [NotNull] IMessagePackWriter writer)
            {
                if (value is TestMessage testMessage)
                {
                    writer.WriteMapHeader(3);
                    var stringConverter = ConverterContext.GetConverter(typeof(string));

                    stringConverter.Write("MessageData", writer);
                    stringConverter.Write(testMessage.MessageData, writer);

                    stringConverter.Write("SendDateTime", writer);
                    ConverterContext.GetConverter(typeof(DateTime)).Write(testMessage.SendDateTime, writer);

                    stringConverter.Write("MessageGuid", writer);
                    ConverterContext.GetConverter(typeof(Guid)).Write(testMessage.MessageGuid, writer);
                }
            }
        }
    }
}
