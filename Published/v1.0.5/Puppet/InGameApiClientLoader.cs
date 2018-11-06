using UnityEngine;

namespace Puppetry.Puppet
{
    public class InGameApiClientLoader : MonoBehaviour
    {
        public void Awake()
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("DriverApiClient is starting...");
                DriverApiClient.Instance.StartClient();
                var puppetProcessor = new GameObject("PuppetProcessor").AddComponent<MainThreadHelper>();
            }
        }
    }
}
