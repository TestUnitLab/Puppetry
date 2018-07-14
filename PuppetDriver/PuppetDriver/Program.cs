using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using Puppetry.PuppetDriver.TcpSocket;

namespace Puppetry.PuppetDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Parallel.Invoke(() => BuildWebHost(args).Run(), StartTcpListner);
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:7111/")
                .Build();

        private static void StartTcpListner()
        {
            PuppetListener.StartListen();
        }
    }
}
