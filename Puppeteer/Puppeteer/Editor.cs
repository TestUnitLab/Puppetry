using Puppetry.Puppeteer.PuppetDriver;
using Puppetry.Puppeteer.Utils;

namespace Puppetry.Puppeteer
{
    public static class Editor
    {
        public static void StartPlayMode()
        {
            Driver.Instance.StartPlayMode();
            
            Wait.For( () => Driver.Instance.IsPlayMode(), () => $"PlayMode was not started in {Configuration.StartPlayModeTimeoutMs / 1000} seconds", Configuration.TimeoutMs);
        }

        public static void StopPlayMode()
        {
            Driver.Instance.StopPlayMode();
        }
    }
}
