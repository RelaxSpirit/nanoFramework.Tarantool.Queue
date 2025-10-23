// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Text;
using nanoFramework.MessagePack.Dto;

namespace nanoFramework.Tarantool.Queue.Model
{
    internal struct QueueSessionIdentify
    {
        private readonly string _identifyString;

        internal QueueSessionIdentify(ArraySegment identifyBytes)
        {
            var buff = (byte[])identifyBytes;

            _identifyString = Convert.ToBase64String(buff);
        }

        internal QueueSessionIdentify(string identifyString)
        {
            _identifyString = identifyString;
        }

        public override string ToString()
        {
            return _identifyString;
        }
    }
}
