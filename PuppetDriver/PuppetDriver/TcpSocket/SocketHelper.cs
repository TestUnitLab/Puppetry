using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Puppetry.PuppetContracts;
using static Puppetry.PuppetContracts.Constants;

namespace Puppetry.PuppetDriver.TcpSocket
{
    internal static class SocketHelper
    {
        internal static bool IsSocketConnected(Socket socket)
        {
            if (socket == null) return false;
            try
            {
                lock (socket)
                {
                    bool part1 = socket.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (socket.Available == 0);

                    if (part1 && part2)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static Dictionary<string, string> SendMessage(Socket socket, Dictionary<string, string> request)
        {
            try
            {
                lock (socket)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var response = new Dictionary<string, string>();
                        socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request, Formatting.Indented) + EndOfMessage));
                        if (socket.Poll(-1, SelectMode.SelectRead))
                        {
                            response = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadMessage(socket));
                        }

                        if (response.Count > 0)
                            return response;
                        Thread.Sleep(500);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        private static string ReadMessage(Socket socket)
        {
            byte[] bytes = new Byte[1024];
            string data = null;

            lock (socket)
            {
                while (true)
                {
                    int bytesRec = socket.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf(EndOfMessage) > -1)
                    {
                        break;
                    }
                }
            }

            if(!(data.Contains(Methods.Ping) && data.Contains(Methods.Pong)))
                Console.WriteLine(data);

            return data.Replace(EndOfMessage, string.Empty);
        }
    }
}
