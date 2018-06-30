using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Assets.Plugins.GamePuppet
{
    [InitializeOnLoad]
    public class PuppetClientLoader
    {
        static PuppetClientLoader()
        {
            Debug.Log("PuppetClient Up and running");
            EditorApplication.update += StartPuppetClient;

            //EditorApplication.playModeStateChanged += InstantiateGOAPI;
        }

        private static void StartPuppetClient()
        {
            EditorApplication.update -= StartPuppetClient;

            var childSocketThread =
                        new Thread(() =>
                        {
                            new PuppetClient().StartClient();
                        });

            childSocketThread.IsBackground = true;
            childSocketThread.Start();
        }

        /*private static void InstantiateGOAPI(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var api = GameObjectUtils.CreateWithComponent<GameObjectAPI>();
                UnityEngine.Object.DontDestroyOnLoad(api);
            }
        }*/
    }

    public class PuppetClient : IDisposable
    {
        private const string EndOfMessage = "<EOF>";
        private TcpClient client;

        public void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            PuppetDriverResponse response = new PuppetDriverResponse();

            // Connect to a remote device.  
            try
            {
                Debug.Log("Puppet client is connecting");
                client = new TcpClient();
                client.Client.Connect(IPAddress.Parse("127.0.0.1"), 6111);

                while (client.Connected)
                {
                    try
                    {
                        if (client.Available > 0)
                        {
                            var message = ReadData(client).Replace(EndOfMessage, string.Empty);
                            var request = JsonUtility.FromJson<PuppetDriverRequest>(message);
                            if (request.method == "registereditor")
                            {
                                Debug.Log("Processing: " + message);
                                response.method = "registereditor";
                                response.result = "unity";
                                client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                                response.Clear();
                            }
                            else if (request.method == "sendkeys" || request.method == "click" || request.method == "startplaymode" || request.method == "stopplaymode" || request.method == "takescreenshot")
                            {
                                Debug.Log("Processing: " + message);
                                response.method = request.method;
                                response.result = "success";
                                client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                                response.Clear();
                            }
                            else if (request.method == "exist" || request.method == "active")
                            {
                                Debug.Log("Processing: " + message);
                                response.method = request.method;
                                response.result = "true";
                                client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                                response.Clear();
                            }

                            else if (request.method == "ping")
                            {
                                response.method = "pong";
                                client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                                response.Clear();
                            }
                            else
                                Debug.Log(message);
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

        public void Dispose()
        {
            if (client != null) ((IDisposable)client).Dispose();
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
}
