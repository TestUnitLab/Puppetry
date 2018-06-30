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
            try
            {
                Console.WriteLine("Starting TCP listener...");
                var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 6111);
                listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                listener.Start();

                while (true)
                {
                    Socket client = listener.AcceptSocket();
                    Console.WriteLine("Connection accepted.");

                    var childSocketThread =
                        new Thread(() =>
                        {
                            IEditorHandler editor;
                            try
                            {
                                var response = SocketHelper.SendMessage(client, new Dictionary<string, string> { { Parameters.Method, Methods.RegisterEditor } });
                                if (response.ContainsKey(Parameters.Method) && response[Parameters.Method] == Methods.RegisterEditor)
                                {
                                    if (response[Parameters.Result] == EditorTypes.UnrealEngine4)
                                    {
                                        editor = new UnrealEngineEditor(client);
                                        ConnectionManager.AddEditor(editor);
                                    }
                                    else if (response[Parameters.Result] == EditorTypes.Unity)
                                    {
                                        editor = new UnrealEngineEditor(client);
                                        ConnectionManager.AddEditor(editor);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Unsupported editor type: {response["editortype"]}. Socket is being closed");
                                        client.Close();
                                    }
                                }
                                var ping = new Dictionary<string, string> { { Parameters.Method, Methods.Ping } };
                                while (SocketHelper.IsSocketConnected(client))
                                {
                                    var pong = SocketHelper.SendMessage(client, ping);
                                    if (!pong.ContainsKey(Parameters.Method) || pong[Parameters.Method] != Methods.Pong)
                                        throw new Exception("Unexpected response from socket");

                                    Thread.Sleep(5000);
                                }
                            }
                            catch (SocketException e)
                            {
                                Console.WriteLine(e);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine(e);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine(e);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }

                        });

                    childSocketThread.Start();
                }

                listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
