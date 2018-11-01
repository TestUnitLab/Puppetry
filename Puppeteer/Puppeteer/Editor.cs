using System.IO;
using Puppetry.Puppeteer.PuppetDriver;

namespace Puppetry.Puppeteer
{
    public static class Editor
    {
        public static void StartPlayMode()
        {
            Driver.Instance.StartPlayMode();
        }

        public static void StopPlayMode()
        {
            Driver.Instance.StopPlayMode();
        }
    }
}
