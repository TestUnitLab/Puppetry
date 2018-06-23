using Puppet.PuppetDriver;
using System.IO;

namespace Puppet
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

        public static void KillSession()
        {
            Driver.Instance.KillSession();
        }
    }
}
