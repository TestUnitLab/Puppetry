using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using PuppetContracts;

namespace PuppetDriver
{
    internal static class SocketHelper
    {
        internal static bool IsSocketConnected(Socket socket)
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

        internal static Dictionary<string, string> SendMessage(Socket socket, Dictionary<string, string> request)
        {
            try
            {
                lock (socket)
                {
                    var response = new Dictionary<string, string>();
                    socket.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request, Formatting.Indented) + Contracts.EndOfMessage));
                    if (socket.Poll(-1, SelectMode.SelectRead))
                    {
                        response = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadMessage(socket));
                    }

                    return response;
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
                    if (data.IndexOf(Contracts.EndOfMessage) > -1)
                    {
                        break;
                    }
                }
            }

            Console.WriteLine(data);
            return data.Replace(Contracts.EndOfMessage, string.Empty);
        }
    }
}
