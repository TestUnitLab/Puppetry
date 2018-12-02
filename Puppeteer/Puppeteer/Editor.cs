using Puppetry.Puppeteer.Utils;

namespace Puppetry.Puppeteer
{
    public static class Editor
    {
        public static void StartPlayMode()
        {
            PuppetDriver.Instance.StartPlayMode();
            
            Wait.For( () => PuppetDriver.Instance.IsPlayMode(), () => $"PlayMode was not started in {Configuration.StartPlayModeTimeoutMs / 1000} seconds", Configuration.TimeoutMs);
        }

        public static void StopPlayMode()
        {
            PuppetDriver.Instance.StopPlayMode();
        }
    }
}
