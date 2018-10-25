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

        public static void MakeScreenshot(string fileName, string folderName)
        {
            var path = Path.Combine(Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), folderName)).FullName, fileName + ".png");

            Driver.Instance.TakeScreenshot(path);
        }

        public static void MakeScreenshot(string fullPath)
        {
            Driver.Instance.TakeScreenshot(fullPath);
        }

        public static void DeletePlayerPref(string key)
        {
            Driver.Instance.DeletePlayerPref(key);
        }

        public static void KillSession()
        {
            Driver.Instance.KillSession();
            Driver.Clear();
        }
    }
}
