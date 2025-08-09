// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.MessagePack.Converters;
using nanoFramework.MessagePack.Stream;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Queue.Model.Enums;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// <see cref="Tarantool"/>.<see cref="Queue"/> tube info.
    /// </summary>
    public partial class TubeInfo
    {
#nullable enable
        internal class TubeInfoConverter : IConverter
        {
            public object? Read([NotNull] IMessagePackReader reader)
            {
                var actual = reader.ReadArrayLength();
                const uint Expected = 5u;

                if (actual != Expected)
                {
                    throw ExceptionHelper.InvalidArrayLength(Expected, actual);
                }

                var intConverter = TarantoolQueueContext.Instance.IntConverter;
                var stringConverter = TarantoolQueueContext.Instance.StringConverter;
                var queueTypeConverter = TarantoolQueueContext.Instance.QueueTypeConverter;
                var tubeCreationOptionsConverter = TarantoolQueueContext.Instance.TubeCreationOptionsConverter;

                TubeInfo tubeInfo = new TubeInfo();

                tubeInfo.Name = (string)(stringConverter.Read(reader) ?? ExceptionHelper.ActualValueIsNullReference());
                tubeInfo.TubeId = (int)(intConverter.Read(reader) ?? ExceptionHelper.ActualValueIsNullReference());
                
                //// Skip space name
                reader.SkipToken();

                var queueType = (QueueType)(queueTypeConverter.Read(reader) ?? ExceptionHelper.ActualValueIsNullReference());

                tubeInfo.CreationOptions = TubeCreationOptions.GetTubeCreationOptions(queueType);
                    
                var creationOption = (TubeCreationOptions)(tubeCreationOptionsConverter.Read(reader) ?? ExceptionHelper.ActualValueIsNullReference());

                foreach (DictionaryEntry option in creationOption)
                {
                    if (option.Key is string optionName)
                    {
                        tubeInfo.CreationOptions[optionName] = option.Value;
                    }
                }

                return tubeInfo;
            }

            public virtual void Write(object? value, [NotNull] IMessagePackWriter writer)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
