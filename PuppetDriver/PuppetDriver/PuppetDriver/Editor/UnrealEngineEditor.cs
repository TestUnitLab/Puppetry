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

        public void SendKeys(string name, string parent)
        {
            throw new NotImplementedException();
        }

        public void Click(string name, string parent)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string name, string parent)
        {
            throw new NotImplementedException();
        }

        public bool Active(string name, string parent)
        {
            throw new NotImplementedException();
        }

        public void StartPlayMode()
        {
            throw new NotImplementedException();
        }

        public void StopPlayMode()
        {
            throw new NotImplementedException();
        }

        public void MakeScreenshot(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
