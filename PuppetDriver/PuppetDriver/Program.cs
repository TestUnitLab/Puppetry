using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            Parallel.Invoke(() => BuildWebHost(args).Run(), StartTcpListner);
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://localhost:7111/")
                .Build();

        private static void StartTcpListner()
        {
            try
            {
                var ipAddress = IPAddress.Parse("127.0.0.1");

                Console.WriteLine("Starting TCP listener...");

                var listener = new TcpListener(ipAddress, 6111);

                listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                listener.Start();

                while (true)
                {
                    Socket client = listener.AcceptSocket();
                    string data = null;
                    byte[] bytes = new Byte[1024];
                    Console.WriteLine("Connection accepted.");

                    var childSocketThread =
                        new Thread(() =>
                        {
                            while (client.Available == 0)
                            {
                                Thread.Sleep(5);
                            }
                            
                            while (IsSocketConnected(client))
                            {
                                try
                                {
                                    while (true)
                                    {
                                        int bytesRec = client.Receive(bytes);
                                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                        if (data.IndexOf("<EOF>") > -1)
                                        {
                                            break;
                                        }
                                    }

                                    byte[] msg = Encoding.ASCII.GetBytes(data.Replace("<EOF>", string.Empty).ToUpperInvariant());
                                    data = null;

                                    client.Send(msg);
                                }
                                catch (SocketException e)
                                {
                                    Console.WriteLine(e);
                                }
                            }

                            client.Close();
                            Console.WriteLine("Socket connection closed.");
                        });

                    childSocketThread.Start();
                }

                listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
                Console.WriteLine("Error: " + e.Message);
                Console.ReadLine();
            }
        }

        private static bool IsSocketConnected(Socket socket)
        {
            bool part1 = socket.Poll(1000, SelectMode.SelectRead);
            bool part2 = (socket.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }
    }
}
