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
    public class PuppetClientLoader
    {
        static PuppetClientLoader()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;

            Debug.Log("PuppetClientLoader is called");

            EditorApplication.playModeStateChanged += InstantiatePuppetProcessor;
            
            EditorApplication.update += StartPuppetClient;
            
            //EditorApplication.playModeStateChanged += RestrictReloadAssemblies;
        }

        static void StartPuppetClient()
        {
            EditorApplication.update -= StartPuppetClient;
            
            //EditorApplication.LockReloadAssemblies();
            PuppetClient.Instance.StartClient();
        }

        static void InstantiatePuppetProcessor(PlayModeStateChange state)
        {
            Debug.Log("InstantiatePuppetProcessor is processed");

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var puppetProcessor = new GameObject("PuppetProcessor").AddComponent<PuppetProcessor>();
                UnityEngine.Object.DontDestroyOnLoad(puppetProcessor);
            }
        }

        static void RestrictReloadAssemblies(PlayModeStateChange stateChange)
        {
            switch (stateChange) 
            {
                case PlayModeStateChange.EnteredPlayMode: 
                {
                    EditorApplication.LockReloadAssemblies();
                    Debug.Log ("Assembly Reload locked as entering play mode");
                    break;
                }
                case PlayModeStateChange.ExitingPlayMode: 
                {
                    Debug.Log ("Assembly Reload unlocked as exiting play mode");
                    EditorApplication.UnlockReloadAssemblies();
                    break;
                }
            }
        }
    }
    
    public class PuppetClient : IDisposable
    {
        private PuppetClient()
        {
            Debug.Log("PuppetClient is intantiated");
        }

        private const string EndOfMessage = "<EOF>";

        private Thread _thread;
        private TcpClient _client;

        private static PuppetClient _instance;

        public static bool IsRun = false;
        public static bool WorkDone = false;

        public static PuppetClient Instance
        {
            get
            {
                return _instance ?? (_instance = new PuppetClient());//new GameObject("PuppetClient").AddComponent<PuppetClient>());
            }
        }
        
        public void StartClient()
        {
            Debug.Log("Thread is started");
            _thread = new Thread(ProcessWork);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void ProcessWork()
        {
            PuppetDriverResponse response = new PuppetDriverResponse();

            // Connect to a remote device.  
            try
            {
                Debug.Log("Puppet client is connecting");
                _client = new TcpClient();
                _client.Client.Connect(IPAddress.Parse("127.0.0.1"), 6111);

                while (_client.Connected)
                {
                    try
                    {
                        if (_client.Available > 0)
                        {
                            var message = ReadData(_client).Replace(EndOfMessage, string.Empty);
                            var request = JsonUtility.FromJson<PuppetDriverRequest>(message);
                            response = PuppetRequestHandler.HandlePuppetDriverRequest(request);
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

