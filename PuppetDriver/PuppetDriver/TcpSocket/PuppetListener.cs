using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Puppetry.PuppetContracts;
using Puppetry.PuppetDriver.Puppet;

namespace Puppetry.PuppetDriver.TcpSocket
{
    internal static class PuppetListener
    {
        internal static void StartListen()
        {
            try
            {
                Console.WriteLine("Starting TCP listener...");
                var listener = new TcpListener(IPAddress.Any, 6111);
                listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                listener.Start();

                while (true)
                {
                    ConnectionManager.RemoveOldPuppets();

                    IPuppetHandler puppet = null;

                    Socket client = listener.AcceptSocket();
                    Console.WriteLine("Connection accepted.");
                    var session = Guid.NewGuid().ToString();

                    var childSocketThread =
                        new Thread(() =>
                        {
                            try
                            {
                                var response = SocketHelper.SendMessage(client, new Dictionary<string, string> { { Parameters.Method, Methods.RegisterEditor }, { Parameters.Session, session } });
                                if (response.ContainsKey(Parameters.Method) && response[Parameters.Method] == Methods.RegisterEditor)
                                {
                                    if (response[Parameters.Result] == EditorTypes.Unity)
                                    {
                                        if (response.ContainsKey(Parameters.Session) && response[Parameters.Session] != session)
                                        {
                                            try
                                            {
                                                puppet = ConnectionManager.ReconnectPuppet(client, response[Parameters.Session]);
                                            }
                                            catch (InvalidOperationException e)
                                            {
                                                Console.WriteLine(e);
                                                puppet = new UnityPuppet(client, response[Parameters.Session]);
                                                ConnectionManager.AddPuppet(puppet);
                                            }
                                        }
                                        else
                                        {
                                            puppet = new UnityPuppet(client, session);
                                            ConnectionManager.AddPuppet(puppet);
                                        }
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
                                    if (!pong.ContainsKey(Parameters.Method) || pong[Parameters.Method] != Methods.Ping || pong[Parameters.Result] != Methods.Pong)
                                        throw new Exception("Unexpected response from socket");
                                    else
                                    {
                                        lock (puppet)
                                        {
                                            puppet.Available = true;
                                            puppet.LastPing = DateTime.UtcNow;
                                        }
                                        Thread.Sleep(1000);
                                    }
                                }

                                ConnectionManager.DisablePuppet(puppet);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }
                            catch (SocketException e)
                            {
                                Console.WriteLine(e);
                                ConnectionManager.DisablePuppet(puppet);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }
                            catch (IOException e)
                            {
                                Console.WriteLine(e);
                                ConnectionManager.DisablePuppet(puppet);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }
                            catch (NullReferenceException e)
                            {
                                Console.WriteLine(e);
                                ConnectionManager.DisablePuppet(puppet);
                                client.Close();
                                Console.WriteLine("Socket connection closed.");
                            }

                        });

                    childSocketThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}
