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

            PuppetListener.Init(settings);

            Parallel.Invoke(() => BuildWebHost(settings).Run(), PuppetListener.StartListen);
        }

        private static IWebHost BuildWebHost(Dictionary<string, string> settings)
        {
            const string PortParameter = "puppeteerport";
            const string BaseUrlParameter = "puppeteerurl";

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
    }
}
