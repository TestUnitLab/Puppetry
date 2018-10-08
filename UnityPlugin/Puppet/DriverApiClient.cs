using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEditor;
using UnityEngine;

namespace Puppetry.Puppet
{
    [InitializeOnLoad]
    public class DriverApiClientLoader
    {
        static DriverApiClientLoader()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            Debug.Log("DriverApiClient is starting...");

            EditorApplication.playModeStateChanged += InstantiatePuppetProcessor;
            
            EditorApplication.update += StartDriverApiClient;
            
            //EditorApplication.playModeStateChanged += RestrictReloadAssemblies;
        }

        static void StartDriverApiClient()
        {
            EditorApplication.update -= StartDriverApiClient;
            
            //EditorApplication.LockReloadAssemblies();
            DriverApiClient.Instance.StartClient();
        }

        static void InstantiatePuppetProcessor(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var puppetProcessor = new GameObject("PuppetProcessor").AddComponent<MainThreadHelper>();
                UnityEngine.Object.DontDestroyOnLoad(puppetProcessor);
            }
        }
    }
    
    public class DriverApiClient : IDisposable
    {
        private const string EndOfMessage = "<EOF>";

        private Thread _thread;
        private TcpClient _client;

        private static DriverApiClient _instance;

        public static bool IsRun = false;
        public static bool WorkDone = false;

        public static DriverApiClient Instance
        {
            get
            {
                return _instance ?? (_instance = new DriverApiClient());//new GameObject("PuppetClient").AddComponent<PuppetClient>());
            }
        }
        
        public void StartClient()
        {
            _thread = new Thread(ProcessWork);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void ProcessWork()
        {
            DriverResponse response = new DriverResponse();

            // Connect to a remote device.  
            try
            {
                _client = new TcpClient();
                _client.Client.Connect(IPAddress.Parse("127.0.0.1"), 6111);

                while (_client.Connected)
                {
                    try
                    {
                        if (_client.Available > 0)
                        {
                            var message = ReadData(_client).Replace(EndOfMessage, string.Empty);
                            var request = JsonUtility.FromJson<DriverRequest>(message);
                            response = DriverHandler.HandleDriverRequest(request);
                            _client.Client.Send(Encoding.ASCII.GetBytes(JsonUtility.ToJson(response) + EndOfMessage));
                            response.Clear();
                        }
                        else
                        {
                            Thread.Sleep(500);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }

                Debug.Log("ApiClient was disconnected");

            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private static string ReadData(TcpClient client)
        {
            string retVal;

            NetworkStream stream = client.GetStream();

            byte[] myReadBuffer = new byte[1024];
            var myCompleteMessage = new StringBuilder();

            do
            {
                var numberOfBytesRead = stream.Read(myReadBuffer, 0, myReadBuffer.Length);

                myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));
            }
            while (stream.DataAvailable);

            retVal = myCompleteMessage.ToString();

            return retVal;
        }

        public void Dispose()
        {
            if (_client != null) ((IDisposable)_client).Dispose();
        }
    }
}

