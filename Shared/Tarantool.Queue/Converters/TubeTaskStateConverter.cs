// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class TubeTaskStateConverter : IConverter
    {
#nullable enable
        public object? Read([NotNull] IMessagePackReader reader)
        {
            var stringConverter = TarantoolQueueContext.Instance.StringConverter;
            string stateString = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
            return (TubeTaskState)stateString[0];
        }

        public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}
