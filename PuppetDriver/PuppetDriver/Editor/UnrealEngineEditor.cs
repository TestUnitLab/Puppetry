using System;

namespace PuppetDriver.Editor
{
    internal class UnrealEngineEditor : IEditorHandler
    {
        public string Identificator { get; }

        public UnrealEngineEditor()
        {
            Identificator = Guid.NewGuid().ToString();
        }

        public void SendKeys(string value, string name, string parent)
        {
        }

        public void Click(string name, string parent)
        {
        }

        public bool Exists(string name, string parent)
        {
            return true;
        }

        public bool Active(string name, string parent)
        {
            return false;
        }

        public void StartPlayMode()
        {

        }

        public void StopPlayMode()
        {
        }

        public void MakeScreenshot(string fullPath)
        {
        }
    }
}
