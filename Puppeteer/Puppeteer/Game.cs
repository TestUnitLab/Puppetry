using System.IO;
using Puppetry.Puppeteer.Exceptions;

namespace Puppetry.Puppeteer
{
    public static class Game
    {
        public static void MakeScreenshot(string fileName, string folderName)
        {
            var path = Path.Combine(Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), folderName)).FullName, fileName + ".png");

            PuppetDriver.Instance.TakeScreenshot(path);
        }

        public static void MakeScreenshot(string fullPath)
        {
            PuppetDriver.Instance.TakeScreenshot(fullPath);
        }

        public static void DeletePlayerPref(string key)
        {
            PuppetDriver.Instance.DeletePlayerPref(key);
        }
        
        public static void DeleteAllPlayerPrefs()
        {
            PuppetDriver.Instance.DeleteAllPlayerPrefs();
        }

        public static float GetFloatPlayerPref(string key)
        {
            var stringResult = PuppetDriver.Instance.GetFloatPlayerPref(key);
            if (float.TryParse(stringResult, out var result))
                return result;

            throw new PuppetryException(stringResult);
        }

        public static int GetIntPlayerPref(string key)
        {
            var stringResult = PuppetDriver.Instance.GetIntPlayerPref(key);
            if (int.TryParse(stringResult, out var result))
                return result;

            throw new PuppetryException(stringResult);
        }

        public static string GetStringPlayerPref(string key)
        {
            return PuppetDriver.Instance.GetStringPlayerPref(key);
        }

        public static void SetFloatPlayerPref(string key, float value)
        {
            PuppetDriver.Instance.SetFloatPlayerPref(key, value.ToString());
        }

        public static void SetIntPlayerPref(string key, int value)
        {
            PuppetDriver.Instance.SetIntPlayerPref(key, value.ToString());
        }

        public static void SetStringPlayerPref(string key, string value)
        {
            PuppetDriver.Instance.SetStringPlayerPref(key, value);
        }

        public static bool PlayerPrefHasKey(string key)
        {
            var stringResult = PuppetDriver.Instance.PlayerPrefHasKey(key);
            if (bool.TryParse(stringResult, out var result))
                return result;

            throw new PuppetryException(stringResult);
        }
    }
}