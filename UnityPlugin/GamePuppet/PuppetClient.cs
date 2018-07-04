using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace GamePuppet
{
    [InitializeOnLoad]
    public class PuppetClientLoader
    {
        static PuppetClientLoader()
        {
            Debug.Log("PuppetClientLoader is called");
            EditorApplication.update += StartPuppetClient;

            EditorApplication.playModeStateChanged += InstantiatePuppetProcessor;
        }

        private static void StartPuppetClient()
        {
            EditorApplication.update -= StartPuppetClient;

            if (!PuppetClient.IsInitalized)
            {
                Debug.Log("Creating new thread");
                PuppetClient.IsInitalized = true;
                var childSocketThread =
                            new Thread(() =>
                            {
                                new PuppetClient().StartClient();
                            });

                childSocketThread.IsBackground = true;
                childSocketThread.Start();
            }
        }

        private static void InstantiatePuppetProcessor(PlayModeStateChange state)
        {
            Debug.Log("InstantiatePuppetProcessor is processed");

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var puppetProcessor = new GameObject().AddComponent<PuppetProcessor>();
                UnityEngine.Object.DontDestroyOnLoad(puppetProcessor);
            }
        }
    }

    public class PuppetClient : IDisposable
    {
        [SerializeField]
        public static bool IsInitalized;
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
                            Debug.Log("Message received: " + message);
                            var request = JsonUtility.FromJson<PuppetDriverRequest>(message);
                            Debug.Log("Request is: " + message + " after the serialization");
                            response = PuppetRequestHandler.HandlePuppetDriverRequest(request);
                            Debug.Log("We are going to send response.method: " + response.method);
                            client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                            Debug.Log("Response was sent");
                            response.Clear();
                        }
                        else
                        {
                            //Debug.Log("Nothing to read.. sleep 100 ms");
                            Thread.Sleep(100);
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
            var myCompleteMessage = new StringBuilder();
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
