// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NANOFRAMEWORK_1_0
using System;
#endif
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;
using nanoFramework.Tarantool.Tests.Mocks.Data;

namespace nanoFramework.Tarantool.Tests.Mocks.Converters
{
    internal class TubeInfoConverterMock : TubeInfo.TubeInfoConverter
    {
#nullable enable
        public override void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is TubeInfoMock tubeInfoMock)
            {
                if (tubeInfoMock.CreationOptions == null)
                {
                    throw new ArgumentNullException();
                }

                writer.WriteArrayHeader(5u);
                var intConverter = ConverterContext.GetConverter(typeof(int));
                var stringConverter = ConverterContext.GetConverter(typeof(string));
                var queueTypeConverter = ConverterContext.GetConverter(typeof(QueueType));
                var tubeCreationOptionsConverter = ConverterContext.GetConverter(typeof(TubeCreationOptions));

                stringConverter.Write(tubeInfoMock.Name, writer);
                intConverter.Write(tubeInfoMock.TubeId, writer);

                //// Skip space name
                stringConverter.Write(tubeInfoMock.Name, writer);

                queueTypeConverter.Write(tubeInfoMock.CreationOptions.QueueType, writer);

                tubeCreationOptionsConverter.Write(tubeInfoMock.CreationOptions, writer);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
