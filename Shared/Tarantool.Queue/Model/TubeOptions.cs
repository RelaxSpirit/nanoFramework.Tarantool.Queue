// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Model
{
    /// <summary>
    /// Base queue tube options class.
    /// </summary>
    public abstract class TubeOptions : IEnumerable
    {
#nullable enable
        private readonly Hashtable _options = new Hashtable();

        /// <summary>
        /// Gets or sets queue tube type.
        /// </summary>
        public abstract QueueType QueueType { get; protected set; }

        /// <summary>
        /// Gets or sets queue tube option.
        /// </summary>
        /// <param name="key">Queue tube option key name.</param>
        /// <returns>Queue tube option value.</returns>
        public object? this[string key]
        {
            get => _options[key];
            set
            {
                ValidateOptionName(key);
                _options[key] = value;
            }
        }

        /// <summary>
        /// Gets queue tube options keys.
        /// </summary>
        public ICollection Keys => _options.Keys;

        /// <summary>
        /// Gets queue tube options values.
        /// </summary>
        public ICollection Values => _options.Keys;

        /// <summary>
        /// Gets options count.
        /// </summary>
        public int Count => _options.Count;

        /// <summary>
        /// Get validate option name exception method.
        /// </summary>
        /// <param name = "optionName" > Option key name.</param>
        /// <returns><see cref = "NotSupportedException" /></returns >
        /// <exception cref= "NotSupportedException" > Returns exception.</exception>
        protected static NotSupportedException GetValidateOptionNameException(string optionName)
        {
            throw new NotSupportedException($"Option '{optionName}' not supported by queue type");
        }

        /// <summary>
        /// Get validate creation option name exception method.
        /// </summary>
        /// <param name="optionName">Option key name.</param>
        /// <returns><see cref="NotSupportedException"/></returns>
        /// <exception cref="NotSupportedException">Returns exception.</exception>
        protected static NotSupportedException GetValidateCreationOptionNameException(string optionName)
        {
            throw new NotSupportedException($"Creation option '{optionName}' not supported by queue tube type");
        }

        /// <summary>
        /// Checks the allowed option names.
        /// </summary>
        /// <param name="optionName">Option name.</param>
        /// <param name="mainOptions">Allowed options.</param>
        /// <returns><see langword="true"/> if option is allowed owner <see langword="false"/>.</returns>
        protected static bool ContainsOptionKey(string optionName, string[] mainOptions)
        {
            foreach (string key in mainOptions)
            {
                if (key == optionName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Add option method.
        /// </summary>
        /// <param name="key">Option key.</param>
        /// <param name="value">Option value.</param>
        public void Add(string key, object value)
        {
            ValidateOptionName(key);
            _options.Add(key, value);
        }

        /// <summary>
        /// Clear options method.
        /// </summary>
        public void Clear()
        {
            _options.Clear();
        }

        /// <summary>
        /// Contains option key method.
        /// </summary>
        /// <param name="key">Option key.</param>
        /// <returns><see langword="true"/> if option is present owner <see langword="false"/>.</returns>
        public bool ContainsKey(string key)
        {
            return _options.Contains(key);
        }

        /// <summary>
        /// Remove option.
        /// </summary>
        /// <param name="key">Option key.</param>
        public void Remove(string key)
        {
            _options.Remove(key);
        }

        /// <summary>
        /// Try get option value method.
        /// </summary>
        /// <param name="key">Option key.</param>
        /// <param name="value">Option value or null.</param>
        /// <returns><see langword="true"/> if option is gets owner <see langword="false"/>.</returns>
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
        {
            value = null;

            if (!ContainsKey(key))
            {
                return false;
            }
            else
            {
                value = this[key];
                return true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _options.GetEnumerator();
        }

        /// <summary>
        /// Validate option key name method.
        /// </summary>
        /// <param name="optionName">Option key name to validate.</param>
        protected abstract void ValidateOptionName(string optionName);

        /// <summary>
        /// Gets TimeSpan value method.
        /// </summary>
        /// <param name="key">Option name.</param>
        /// <returns><see cref="TimeSpan"/> option value by option name.</returns>
        protected TimeSpan GetTimeSpanValue(string key)
        {
            if (TryGetValue(key, out object? value) && value != null)
            {
                return TimeSpan.FromSeconds((long)value);
            }
            else
            {
                return TimeSpan.MinValue;
            }
        }

        /// <summary>
        /// Sets TimeSpan value method.
        /// </summary>
        /// <param name="key">Option key name.</param>
        /// <param name="value">Option value.</param>
        protected void SetTimeSpanValue(string key, TimeSpan value)
        {
            if (value != TimeSpan.MinValue)
            {
                this[key] = (long)value.TotalSeconds;
            }
            else
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Override base method <see cref="object.ToString()"/>
        /// </summary>
        /// <returns>Options key value string.</returns>
        public override string ToString()
        {
            if (this.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('{');
                foreach (DictionaryEntry value in _options)
                {
                    sb.Append(value.Key);
                    sb.Append('=');

                    if (value.Value is string)
                    {
                        sb.Append($"'{value.Value}'");
                    }

                    sb.Append(value.Value?.ToString());
                    sb.Append(", ");
                }

                if (sb.Length > 1)
                {
                    sb.Remove(sb.Length - 2, 2);
                }

                sb.Append('}');
                return sb.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
