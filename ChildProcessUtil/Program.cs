using System;
using System.Collections.Generic;
using Nancy;
using Nancy.Hosting.Self;

namespace ChildProcessUtil
{
    public class Program
    {
        private const string HttpAddress = "http://localhost:";
        private static NancyHost host;
        private static void Main(string[] args)
        {
            StartServer(30197);
        }

        public static void StartServer(int port)
        {
            var hostConfigs = new HostConfiguration
            {
                UrlReservations = new UrlReservations {CreateAutomatically = true}
            };

            var uriString = HttpAddress + port;
            ProcessModule.ActiveProcesses = new List<int>();
            host = new NancyHost(new Uri(uriString), new DefaultNancyBootstrapper(), hostConfigs);
            host.Start();
        }

        public static void StopServer(int port)
        {
            host.Stop();
        }
    }
}