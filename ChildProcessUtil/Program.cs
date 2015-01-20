using System;
using System.Collections.Generic;
using System.Threading;
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
            if (args.Length != 2)
            {
                Console.WriteLine("usage: ChildProcessUtil.exe serverPort mainProcessId");
                return;
            }
            StartServer(int.Parse(args[0]), int.Parse(args[1]));
        }

        public static void StartServer(int port, int mainProcessId)
        {
            var hostConfigs = new HostConfiguration
            {
                UrlReservations = new UrlReservations {CreateAutomatically = true}
            };

            var uriString = HttpAddress + port;
            ProcessModule.ActiveProcesses = new List<int>();
            host = new NancyHost(new Uri(uriString), new DefaultNancyBootstrapper(), hostConfigs);
            host.Start();
            new MainProcessWatcher(mainProcessId);
            Thread.Sleep(Timeout.Infinite);
        }

        internal static void StopServer(int port)
        {
            host.Stop();
        }
    }
}