using System.IO;
using Puppetry.Puppeteer.Exceptions;

namespace Puppetry.Puppeteer
{
    public static class Game
    {
        public static void MakeScreenshot(string fileName, string folderName)
        {
            var path = Path.Combine(Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), folderName)).FullName, fileName);

            PuppetryDriver.Instance.TakeScreenshot(path);
        }

        public static void MakeScreenshot(string fullPath)
        {
            PuppetryDriver.Instance.TakeScreenshot(fullPath);
        }

        public static void DeletePlayerPref(string key)
        {
            PuppetryDriver.Instance.DeletePlayerPref(key);
        }
        
        public static void DeleteAllPlayerPrefs()
        {
            PuppetryDriver.Instance.DeleteAllPlayerPrefs();
        }

        public static float GetFloatPlayerPref(string key)
        {
            var stringResult = PuppetryDriver.Instance.GetFloatPlayerPref(key);
            if (float.TryParse(stringResult, out var result))
                return result;

            throw new PuppetryException(stringResult);
        }

        public static int GetIntPlayerPref(string key)
        {
            var stringResult = PuppetryDriver.Instance.GetIntPlayerPref(key);
            if (int.TryParse(stringResult, out var result))
                return result;

            throw new PuppetryException(stringResult);
        }

        public static string GetStringPlayerPref(string key)
        {
            return PuppetryDriver.Instance.GetStringPlayerPref(key);
        }

        public static void SetFloatPlayerPref(string key, float value)
        {
            PuppetryDriver.Instance.SetFloatPlayerPref(key, value.ToString());
        }

        public static void SetIntPlayerPref(string key, int value)
        {
            PuppetryDriver.Instance.SetIntPlayerPref(key, value.ToString());
        }

        public static void SetStringPlayerPref(string key, string value)
        {
            PuppetryDriver.Instance.SetStringPlayerPref(key, value);
        }

        public static bool PlayerPrefHasKey(string key)
        {
            var stringResult = PuppetryDriver.Instance.PlayerPrefHasKey(key);
            if (bool.TryParse(stringResult, out var result))
                return result;

            throw new PuppetryException(stringResult);
        }

        public static string GetSceneName()
        {
            return PuppetryDriver.Instance.GetSceneName();
        }

        public static string OpenScene(string scene)
        {
            return PuppetryDriver.Instance.OpenScene(scene);
        }

        public static void ExecuteCustomMethod(string method, string value = null)
        {
            PuppetryDriver.Instance.GameCustomMethod(method, value);
        }
    }
}