// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Threading;
using nanoFramework.Tarantool.Client.Interfaces;
using nanoFramework.Tarantool.Helpers;
using nanoFramework.Tarantool.Model;
using nanoFramework.Tarantool.Model.Enums;
using nanoFramework.Tarantool.Model.Requests;
using nanoFramework.Tarantool.Queue.Client.Interfaces;
using nanoFramework.Tarantool.Queue.Model;
using nanoFramework.Tarantool.Queue.Model.Enums;

namespace nanoFramework.Tarantool.Queue.Client
{
    internal class Queue : IAdminQueue
    {
        private readonly IBox _box;
        private readonly Hashtable _tubeByName = new Hashtable();

        internal Queue(QueueClientOptions clientOptions)
        {
            clientOptions.RequestTimeout = Timeout.Infinite;

            _box = TarantoolContext.Connect(clientOptions);

            Require = clientOptions.Require;

            var responseData = _box.Call($"{Require}.identify", typeof(string[]));
            if (responseData != null && responseData.Data[0] is string guidString)
            {
                SessionUuid = guidString;
            }

            var versionResponse = _box.Eval($"return {Require}._VERSION", typeof(string[]));
            if (versionResponse != null && versionResponse.Data[0] is string version)
            {
                Version = version;
            }
        }

        private static string QueueTypeToString(QueueType queueType)
        {
            switch (queueType)
            {
                case QueueType.Fifo:
                    return "fifo";
                case QueueType.FifoTtl:
                    return "fifottl";
                case QueueType.LimFifoTtl:
                    return "limfifottl";
                case QueueType.Utube:
                    return "utube";
                case QueueType.UtubeTtl:
                    return "utubettl";
                default:
                    return "customtube";
            }
        }

        internal string Require { get; private set; }

#nullable enable
        public string SessionUuid { get; } = string.Empty;

        public string Version { get; } = string.Empty;

        public ITube this[string tubeName]
        {
            get
            {
                var tube = _tubeByName[tubeName];
                if (tube == null)
                {
                    var tubeInfo = GetTubeInfo(tubeName) ?? throw new ArgumentException($"Tube name {tubeName} das not exist.");
                    
                    tube = new Tube(tubeInfo, _box, Require);
                }

                return (ITube)tube;
            }
        }

        public QueueState GetState()
        {
            var stateResponse = _box.Call($"{Require}.state");

            if (stateResponse != null && stateResponse.Data[0] is string stateString)
            {
                return (QueueState)(ushort)stateString.ToLower()[0];
            }
            else
            {
                throw ExceptionHelper.ActualValueIsNullReference();
            }
        }

        public QueueStatistic GetStatistics()
        {
            var stateResponse = _box.Call($"{Require}.statistics", typeof(QueueStatistic[]));

            if (stateResponse == null)
            {
                throw ExceptionHelper.ActualValueIsNullReference();
            }
            else
            {
                return (QueueStatistic)stateResponse.Data[0];
            }
        }

        public ITube? CreateTube(string tubeName, TubeCreationOptions options)
        {
            if (_tubeByName.Contains(tubeName))
            {
                if (options.IfNotExists)
                {
                    throw new ArgumentException($"Tube {tubeName} already exists.");
                }
            }
            else
            {
                _box.Eval($"{Require}.create_tube('{tubeName}', '{QueueTypeToString(options.QueueType)}'{(options.Count > 0 ? $", {options}" : string.Empty)})");

                var tubeInfo = GetTubeInfo(tubeName) ?? throw new Exception($"Error create tube '{tubeName}' at options {options}.");
                _tubeByName[tubeName] = new Tube(tubeInfo, _box, Require);
            }

            return _tubeByName[tubeName] as ITube;
        }

        public void DeleteTube(string tubeName)
        {
            _box.Call($"{Require}.tube.{tubeName}:drop");
            _tubeByName.Remove(tubeName);
        }

        public void Dispose()
        {
            _box.Dispose();
        }

        private TubeInfo? GetTubeInfo(string tubeName)
        {
            var queueSpaceId = _box.Schema["_queue"].Id;
            var queueTubeIndexId = _box.Schema["_queue"]["tube"].Id;

            var request = new SelectRequest(queueSpaceId, queueTubeIndexId, 1, 0, Iterator.Eq, TarantoolTuple.Create(tubeName));

            var dataResponse = _box.TarantoolConnection.SendRequest(request, TimeSpan.FromSeconds(30), typeof(TubeInfo[]));

            if (dataResponse != null && dataResponse.Data is TubeInfo[] tubes && tubes.Length > 0)
            {
                return tubes[0];
            }
            else
            {
                return null;
            }
        }
    }
}
