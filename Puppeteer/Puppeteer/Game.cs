using System.IO;
using Puppetry.Puppeteer.PuppetDriver;

namespace Puppetry.Puppeteer
{
    public static class Game
    {
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
        
        public static void DeleteAllPlayerPrefs()
        {
            Driver.Instance.DeleteAllPlayerPrefs();
        }
    }
}