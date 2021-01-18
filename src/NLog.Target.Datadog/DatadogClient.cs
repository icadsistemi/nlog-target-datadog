// Unless explicitly stated otherwise all files in this repository are licensed
// under the Apache License Version 2.0.
// This product includes software developed at Datadog (https://www.datadoghq.com/).
// Copyright 2019 Datadog, Inc.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NLog.Target.Datadog
{
    public interface IDatadogClient
    {
        /// <summary>Send payload to Datadog logs-backend synchronously.</summary>
        void Write(IReadOnlyCollection<string> events);

        /// <summary>Send payload to Datadog logs-backend asynchronously.</summary>
        Task WriteAsync(IReadOnlyCollection<string> events);

        /// <summary>
        ///     Cleanup existing resources.
        /// </summary>
        void Close();
    }

    public abstract class DataDogClient : IDatadogClient, IDisposable
    {
        protected DataDogClient(int maxRetries, int maxBackoff)
        {
            MaxRetries = maxRetries;
            MaxBackoff = maxBackoff;
        }


        /// <summary>Maximum retries before giving in posting batch.</summary>
        protected int MaxRetries { get; set; }

        /// <summary>Maximum waiting time before posting same failed batch again.</summary>
        protected int MaxBackoff { get; set; }

        public abstract void Dispose();
        public abstract void Write(IReadOnlyCollection<string> events);
        public abstract Task WriteAsync(IReadOnlyCollection<string> events);
        public abstract void Close();
    }
}