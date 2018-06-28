using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Program
{
    private const string EndOfMessage = "<EOF>";

    public static void StartClient()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];
        Dictionary<string, string> response = new Dictionary<string, string>(); ;

        // Connect to a remote device.  
        try
        {
            TcpClient client = new TcpClient();
            client.Client.Connect(IPAddress.Parse("127.0.0.1"), 6111);

            while (client.Connected)
            {
                try
                {
                    //client.Client.Send(Encoding.ASCII.GetBytes("{\"TID\":1111,\"blabla\":{}}" + "<EOF>"));
                    if (client.Available > 0)
                    {
                        var message = ReadData(client).Replace(EndOfMessage, string.Empty);
                        var request = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                        if (request.ContainsKey("method") && request["method"] == "registereditor")
                        {
                            response.Add("method", "registereditor");
                            response.Add("editortype", "ue4");
                            client.Client.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(response, Formatting.Indented) + EndOfMessage));
                            response.Clear();
                            Console.WriteLine("registerrequest method processed");
                        }
                        else if (request.ContainsKey("method") && (request["method"] == "sendkeys" || request["method"] == "click" || request["method"] == "startplaymode" || request["method"] == "stopplaymode" || request["method"] == "takescreenshot"))
                        {
                            response.Add("method", request["method"]);
                            response.Add("result", "success");
                            client.Client.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(response, Formatting.Indented) + EndOfMessage));
                            response.Clear();
                            Console.WriteLine("Void method processed");
                        }
                        else if (request.ContainsKey("method") && (request["method"] == "exist" || request["method"] == "active"))
                        {
                            response.Add("method", request["method"]);
                            response.Add("result", "true");
                            client.Client.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(response, Formatting.Indented) + EndOfMessage));
                            response.Clear();
                            Console.WriteLine("Bool method processed");
                        }

                        else if (request.ContainsKey("method") && request["method"] == "ping")
                        {
                            response.Add("method", "pong");
                            client.Client.Send(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(response, Formatting.Indented) + EndOfMessage));
                            response.Clear();
                            Console.WriteLine("ping method processed");
                        }
                        Console.WriteLine(message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main(String[] args)
    {
        StartClient();
        return 0;
    }

    private static string ReadData(TcpClient client)
    {
        string retVal;
        byte[] data = new byte[1024];

        NetworkStream stream = client.GetStream();


        byte[] myReadBuffer = new byte[1024];
        StringBuilder myCompleteMessage = new StringBuilder();
        int numberOfBytesRead = 0;


        do
        {
            numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

            myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

        }
        while (stream.DataAvailable);



        retVal = myCompleteMessage.ToString();


        return retVal;
    }

}