using System;
using Nancy;
using Nancy.Hosting.Self;

namespace ChildProcessUtil
{
    internal class Program
    {
        const string HttpAddress = "http://localhost:30197";
      
        private static void Main(string[] args)
        {
            var hostConfigs = new HostConfiguration
            {
                UrlReservations = new UrlReservations {CreateAutomatically = true}
            };

            using (var host = new NancyHost(new Uri(HttpAddress), new DefaultNancyBootstrapper(), hostConfigs))
            {
                host.Start();
                Console.ReadLine();
            }
        }
    }
}