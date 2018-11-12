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
        }

        static void StartDriverApiClient()
        {
            EditorApplication.update -= StartDriverApiClient;

            DriverApiClient.Instance.StartClient();
        }

        static void InstantiatePuppetProcessor(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var puppetProcessor = new GameObject("PuppetProcessor").AddComponent<MainThreadQueue>();
                Object.DontDestroyOnLoad(puppetProcessor);
            }
        }
    }
}
