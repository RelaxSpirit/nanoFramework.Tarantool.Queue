// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Queue.Model;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class PutTubeOptionsConverter : IConverter
    {
#nullable enable
        public virtual object? Read([NotNull] IMessagePackReader reader)
        {
            throw new NotImplementedException();
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is TubeOptions tubeOptions)
            {
                writer.WriteMapHeader((uint)tubeOptions.Count);

                var stringConverter = ConverterContext.GetConverter(typeof(string));

                foreach (DictionaryEntry dictionaryEntry in tubeOptions)
                {
                    if (dictionaryEntry.Value == null)
                    {
                        throw new ArgumentNullException();
                    }

                    stringConverter.Write(dictionaryEntry.Key.ToString(), writer);
                    writer.Write(MessagePackSerializer.Serialize(dictionaryEntry.Value));
                }
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
