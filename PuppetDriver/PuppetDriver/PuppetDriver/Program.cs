using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PuppetDriver.Editor;

namespace PuppetDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionManager.AddEditor(new UnrealEngineEditor());
            BuildWebHost(args).Run();
            //Parallel.Invoke(() => BuildWebHost(args).Run(), StartTcpListner);
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:7111/")
                .Build();
    }
}
