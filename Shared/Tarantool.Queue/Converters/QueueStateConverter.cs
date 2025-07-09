// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class QueueStateConverter : IConverter
    {
#nullable enable
        public object? Read([NotNull] IMessagePackReader reader)
        {
            var stringConverter = ConverterContext.GetConverter(typeof(string));
            var stateString = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
            return (QueueState)(ushort)stateString.ToLower()[0];
        }

        public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
