using UnityEngine;

namespace Puppetry.Puppet
{
    public class InGameApiClientLoader : MonoBehaviour
    {
        public void Init(int port)
        {
            DriverApiClient.Instance.Port = port;
            Init();
        }
        
        public void Init(string ipAddress, int port)
        {
            DriverApiClient.Instance.IpAddress = ipAddress;
            DriverApiClient.Instance.Port = port;
            Init();
        }
        
        public void Init(string ipAddress)
        {
            DriverApiClient.Instance.IpAddress = ipAddress;
            Init();
        }
        
        public void Init()
        {
            Debug.Log("DriverApiClient is starting...");
            DriverApiClient.Instance.StartClient();
            var puppetProcessor = new GameObject("PuppetProcessor").AddComponent<MainThreadQueue>();
            DontDestroyOnLoad(puppetProcessor);
        }
    }
}
