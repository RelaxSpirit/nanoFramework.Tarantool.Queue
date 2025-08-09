// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Converters
{
    internal class TubeCreationOptionsConverter : IConverter
    {
#nullable enable
        public object? Read([NotNull] IMessagePackReader reader)
        {
            var mapLength = reader.ReadMapLength();
            if (mapLength == uint.MaxValue)
            {
                return null;
            }

            var stringConverter = TarantoolQueueContext.Instance.StringConverter;
            var boolConverter = TarantoolQueueContext.Instance.BoolConverter;
            var ulongConverter = TarantoolQueueContext.Instance.UlongConverter;

            TubeCreationOptions tubeCreationOptions = TubeCreationOptions.GetTubeCreationOptions(QueueType.CustomTube);

            for (var i = 0; i < mapLength; i++)
            {
                var optionKey = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());

                switch (optionKey)
                {
                    case TubeCreationOptions.IfNotExistsConst:
                        tubeCreationOptions.IfNotExists = (bool)(boolConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                        break;
                    case TubeCreationOptions.CapacityConst:
                        tubeCreationOptions.Capacity = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                        break;
                    case TubeCreationOptions.TemporaryConst:
                        tubeCreationOptions.Temporary = (bool)(boolConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                        break;
                    case TubeCreationOptions.StorageModeConst:
                        tubeCreationOptions.StorageMode = (string)(stringConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                        break;
                    case TubeCreationOptions.TtlConst:
                    case TubeCreationOptions.TtrConst:
                    case TubeCreationOptions.PriorityConst:
                        tubeCreationOptions[optionKey] = (ulong)(ulongConverter.Read(reader) ?? throw ExceptionHelper.ActualValueIsNullReference());
                        break;
                    default:
                        reader.SkipToken();
                        break;
                }
            }

            return tubeCreationOptions;
        }

        public void Write(object? value, [NotNull] IMessagePackWriter writer)
        {
            if (value is TubeCreationOptions tubeCreationOptions)
            {
                writer.WriteMapHeader((uint)tubeCreationOptions.Count);
                var stringConverter = TarantoolQueueContext.Instance.StringConverter;
                var boolConverter = TarantoolQueueContext.Instance.BoolConverter;
                var ulongConverter = TarantoolQueueContext.Instance.UlongConverter;

                foreach (DictionaryEntry option in tubeCreationOptions)
                {
                    stringConverter.Write(option.Key, writer);
                    switch (option.Key)
                    {
                        case TubeCreationOptions.CapacityConst:
                            ulongConverter.Write(option.Value, writer);
                            break;
                        case TubeCreationOptions.IfNotExistsConst:
                        case TubeCreationOptions.TemporaryConst:
                            boolConverter.Write(option.Value, writer);
                            break;
                         case TubeCreationOptions.StorageModeConst:
                            stringConverter.Write(option.Value, writer);
                            break;
                        case TubeCreationOptions.TtlConst:
                        case TubeCreationOptions.TtrConst:
                        case TubeCreationOptions.PriorityConst:
                            ulongConverter.Write(option.Value, writer);
                            break;
                    }
                }
            }
        }
    }
}
