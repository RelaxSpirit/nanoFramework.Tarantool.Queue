// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class QueueTypeConverter : IConverter
    {
#nullable enable
        public object? Read([NotNull] IMessagePackReader reader)
        {
            var stringConverter = ConverterContext.GetConverter(typeof(string));
            var stateString = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
            
            switch (stateString)
            {
                case "fifo":
                    return QueueType.Fifo;
                case "fifottl":
                    return QueueType.FifoTtl;
                case "limfifottl":
                    return QueueType.LimFifoTtl;
                case "utube":
                    return QueueType.Utube;
                case "utubettl":
                    return QueueType.UtubeTtl;
                default:
                    return QueueType.CustomTube;
            }
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is QueueType queueType)
            {
                var stringConverter = ConverterContext.GetConverter(typeof(string));

                switch (queueType)
                {
                    case QueueType.Fifo:
                        stringConverter.Write("fifo", writer);
                        break;
                    case QueueType.FifoTtl:
                        stringConverter.Write("fifottl", writer);
                        break;
                    case QueueType.LimFifoTtl:
                        stringConverter.Write("limfifottl", writer);
                        break;
                    case QueueType.Utube:
                        stringConverter.Write("utube", writer);
                        break;
                    case QueueType.UtubeTtl:
                        stringConverter.Write("utubettl", writer);
                        break;
                    default:
                        stringConverter.Write("custom", writer);
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
