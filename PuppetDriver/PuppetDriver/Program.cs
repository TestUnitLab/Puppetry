using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using PuppetDriver.Editor;
using PuppetDriver.TcpSocket;
using static PuppetContracts.Contracts;

namespace PuppetDriver
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
