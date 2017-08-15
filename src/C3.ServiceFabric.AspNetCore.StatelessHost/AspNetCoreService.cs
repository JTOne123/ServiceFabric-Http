﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Fabric;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace C3.ServiceFabric.AspNetCore.StatelessHost
{
    public class AspNetCoreService : IStatelessServiceInstance
    {
        private readonly IWebHost _webHost;

        public AspNetCoreService(IWebHost webHost)
        {
            if (webHost == null)
                throw new ArgumentNullException(nameof(webHost));

            _webHost = webHost;
        }

        public void Initialize(StatelessServiceInitializationParameters initializationParameters)
        {
            Console.WriteLine("AspNetCoreService: Initialize");
        }

        public Task<string> OpenAsync(IStatelessServicePartition partition, CancellationToken cancellationToken)
        {
            Console.WriteLine("AspNetCoreService: OpenAsync");

            _webHost.Start();

            Console.WriteLine("AspNetCoreService: WebHost started");

            var serverAddressesFeature = _webHost.ServerFeatures.Get<IServerAddressesFeature>();
            string addresses = string.Join(";", serverAddressesFeature.Addresses);

            Console.WriteLine("AspNetCoreService: Returning Addresses " + addresses);
            return Task.FromResult(addresses);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("AspNetCoreService: CloseAsync");

            Stop();
            return Task.FromResult(true);
        }

        public void Stop()
        {
            _webHost?.Dispose();
        }

        public void Abort()
        {
            Console.WriteLine("AspNetCoreService: Abort");

            Stop();
        }

        public static string GetServiceTypeName()
        {
            string applicationName = Assembly.GetEntryAssembly().GetName().Name;

            return string.Format("{0}Type", applicationName);
        }
    }
}