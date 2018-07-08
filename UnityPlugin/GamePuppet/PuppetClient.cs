using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GamePuppet.Plugins
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

            PuppetClient.Instance.Start();
        }

        private static void InstantiatePuppetProcessor(PlayModeStateChange state)
        {
            Debug.Log("InstantiatePuppetProcessor is processed");

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var puppetProcessor = new GameObject("PuppetProcessor").AddComponent<PuppetProcessor>();
                UnityEngine.Object.DontDestroyOnLoad(puppetProcessor);
            }
        }
    }
    
    public class PuppetClient : IDisposable
    {
        private PuppetClient()
        {
            Debug.Log("PuppetClient instance is created");
        }

        private const string EndOfMessage = "<EOF>";

        public Thread Thread;
        private TcpClient client;

        static PuppetClient instance;
        public static PuppetClient Instance
        {
            get
            {
                return instance ?? (instance = new PuppetClient());//new GameObject("PuppetClient").AddComponent<PuppetClient>());
            }
        }
        
        public async void Start()
        {
            Debug.Log("Thread is started");
            Thread = new Thread(new ThreadStart(ProcessWork));
            Thread.IsBackground = true;
            Thread.Start();

            await Task.Run(() => !Thread.IsAlive);

            Debug.Log("Thread was dead");
        }

        public void ProcessWork()
        {
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
                            //Debug.Log("Message received: " + message);
                            var request = JsonUtility.FromJson<PuppetDriverRequest>(message);
                            //Debug.Log("Request is: " + message + " after the serialization");
                            response = PuppetRequestHandler.HandlePuppetDriverRequest(request);
                            //Debug.Log("We are going to send response.method: " + response.method);
                            client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                            //Debug.Log("Response was sent");
                            response.Clear();
                        }
                        else
                        {
                            //Debug.Log("Nothing to read.. sleep 100 ms");
                            Thread.Sleep(500);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }

                Debug.Log("Client was disconnected");

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
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

        public void Dispose()
        {
            if (client != null) ((IDisposable)client).Dispose();
        }
    }
}
