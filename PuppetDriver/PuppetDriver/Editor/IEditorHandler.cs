
using System.Net.Sockets;

namespace PuppetDriver.Editor
{
    interface IEditorHandler
    {
        string Session { get;}

        Socket Socket { get; set; }

        EditorResponse Click(string name, string parent);

        EditorResponse SendKeys(string value, string name, string parent);

        EditorResponse Exists(string name, string parent);

        EditorResponse Active(string name, string parent);

        EditorResponse StartPlayMode();

        EditorResponse StopPlayMode();

        EditorResponse MakeScreenshot(string fullPath);
    }
}
