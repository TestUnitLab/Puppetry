using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Puppetry.PuppetryDriver.TcpSocket;

namespace Puppetry.PuppetryDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = Configuration.ProcessComandLineArguments(args);

            Parallel.Invoke(() => BuildWebHost(settings).Run(), StartTcpListner);
        }

        private static IWebHost BuildWebHost(Dictionary<string, string> settings)
        {
            const string PortParameter = "port";
            const string BaseUrlParameter = "baseurl";

            string baseUrl = "http://127.0.0.1";
            string port = "7111";

            if (settings.Count > 0)
            {
                if (settings.ContainsKey(PortParameter))
                    port = settings[PortParameter];
                if (settings.ContainsKey(BaseUrlParameter))
                    baseUrl = settings[BaseUrlParameter];
            }

            return WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls($"{baseUrl}:{port}/")
                .Build();
        }

        private static void StartTcpListner()
        {
            PuppetListener.StartListen();
        }
    }
}
