// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Converters;
using nanoFramework.Tarantool.Queue.Model;

namespace nanoFramework.Tarantool.Tests.Mocks.Converters
{
    internal class PutTubeOptionsConverterMock : PutTubeOptionsConverter
    {
#nullable enable
        public override object? Read([NotNull] IMessagePackReader reader)
        {
            var length = reader.ReadMapLength();

            var hashtableConverter = ConverterContext.GetConverter(typeof(Hashtable));
            var htOptions = (Hashtable)(hashtableConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());

            PuttingCustomTubeOptions options = new PuttingCustomTubeOptions();

            foreach (DictionaryEntry dictionaryEntry in htOptions)
            {
                options[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
            }

            return options;
        }
    }
}
