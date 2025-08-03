// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class QueueSessionIdentifyConverter : IConverter
    {
#nullable enable
        public object? Read([NotNull] IMessagePackReader reader)
        {
            var length = reader.ReadArrayLength();
            if (length != 1)
            {
                throw ExceptionHelper.InvalidArrayLength(1, length);
            }

            return new QueueSessionIdentify(reader.ReadToken() ?? throw ExceptionHelper.ActualValueIsNullReference());
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is QueueSessionIdentify sessionIdentify)
            {
                writer.WriteArrayHeader(1);
                byte[] identifyBytes = Convert.FromBase64String(sessionIdentify.ToString());
                byte stringType = (byte)((byte)DataTypes.FixStr + identifyBytes.Length); 
                writer.Write(stringType);
                writer.Write(identifyBytes);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
