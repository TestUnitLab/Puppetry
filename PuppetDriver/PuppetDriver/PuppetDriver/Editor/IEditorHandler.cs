
namespace PuppetDriver.Editor
{
    interface IEditorHandler
    {
        string Identificator { get;}

        void Click(string name, string parent);

        void SendKeys(string value, string name, string parent);

        bool Exists(string name, string parent);

        bool Active(string name, string parent);

        void StartPlayMode();

        void StopPlayMode();

        void MakeScreenshot(string fullPath);
    }
}
