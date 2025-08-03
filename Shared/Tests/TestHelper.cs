// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using nanoFramework.MessagePack;
#if NANOFRAMEWORK_1_0
using nanoFramework.Tarantool.Tests.Mocks;
#endif
using static nanoFramework.Tarantool.Queue.Tests.TestMessage;

namespace nanoFramework.Tarantool.Queue.Tests
{
    internal static class TestHelper
    {
        static TestHelper()
        {
            ConverterContext.Add(typeof(TestMessage), new TestMessageConverter());
        }

        /// <summary>
        /// <see cref="Tarantool"/> instance ip or host name.
        /// </summary>
#nullable enable
        internal const string TarantoolHostIp = "192.168.1.116";

        /// <summary>
        /// Gets client options method.
        /// </summary>
        /// <param name="isReadSchemaOnConnect">Determines whether to load schema information during connection.</param>
        /// <param name="isReadBoxInfoOnConnect">Determines whether to load box info information during connection.</param>
        /// <param name="writeStreamBufferSize">Write buffer size. Default 8192 bytes.</param>
        /// <param name="readStreamBufferSize">Read buffer size. Default 8192 bytes.</param>
        /// <param name="userData">User name and password. Default <see langword="null"/></param>
        /// <returns>New <see cref="QueueClientOptions"/> instance.</returns>
        internal static QueueClientOptions GetClientOptions(
            bool isReadSchemaOnConnect,
            bool isReadBoxInfoOnConnect,
            int writeStreamBufferSize = 8192,
            int readStreamBufferSize = 8192,
            string? userData = null)
        {
            string replicationSource = $"{TarantoolHostIp}:3301";

            if (userData != null)
            {
                replicationSource = $"{userData}@{replicationSource}";
            }

            QueueClientOptions clientOptions = new QueueClientOptions(replicationSource);
            clientOptions.ConnectionOptions.ReadSchemaOnConnect = isReadSchemaOnConnect;
            clientOptions.ConnectionOptions.ReadBoxInfoOnConnect = isReadBoxInfoOnConnect;
#if NANOFRAMEWORK_1_0
            clientOptions.ConnectionOptions.WriteStreamBufferSize = writeStreamBufferSize > 512 ? 512 : writeStreamBufferSize;
            clientOptions.ConnectionOptions.ReadStreamBufferSize = readStreamBufferSize > 512 ? 512 : readStreamBufferSize;
            clientOptions.GetNetworkStream = TarantoolQueueMockContext.Instanse.GetTarantoolStreamMock;
            
#else
            clientOptions.ConnectionOptions.WriteStreamBufferSize = writeStreamBufferSize;
            clientOptions.ConnectionOptions.ReadStreamBufferSize = readStreamBufferSize;
#endif
            return clientOptions;
        }
    }
}
