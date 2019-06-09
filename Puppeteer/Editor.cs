using Puppetry.Puppeteer.Utils;

namespace Puppetry.Puppeteer
{
    public static class Editor
    {
        public static void StartPlayMode()
        {
            PuppetryDriver.Instance.StartPlayMode();
            
            Wait.For( () => PuppetryDriver.Instance.IsPlayMode(), () => $"PlayMode was not started in {Configuration.StartPlayModeTimeoutMs / 1000} seconds", Configuration.TimeoutMs);
        }

        public static void StopPlayMode()
        {
            PuppetryDriver.Instance.StopPlayMode();
        }
    }
}
