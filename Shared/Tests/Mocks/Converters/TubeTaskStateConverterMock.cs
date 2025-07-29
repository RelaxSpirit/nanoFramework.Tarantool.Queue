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
    internal class TubeTaskStateConverterMock : TubeTaskStateConverter
    {
#nullable enable
        public override void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is TubeTaskState tubeTaskState)
            {
                var stringConverter = ConverterContext.GetConverter(typeof(string));

                switch (tubeTaskState)
                {
                    case TubeTaskState.DELAYED:
                        stringConverter.Write("~", writer);
                        break;
                    case TubeTaskState.BURIED:
                        stringConverter.Write("!", writer);
                        break;
                    case TubeTaskState.TAKEN:
                        stringConverter.Write("t", writer);
                        break;
                    case TubeTaskState.DONE:
                        stringConverter.Write("-", writer);
                        break;
                    default:
                        stringConverter.Write("r", writer);
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
