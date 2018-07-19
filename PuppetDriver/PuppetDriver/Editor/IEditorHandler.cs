
using System.Net.Sockets;

namespace Puppetry.PuppetDriver.Editor
{
    interface IEditorHandler
    {
        string Session { get;}

        Socket Socket { get; set; }

        EditorResponse Click(string root, string name, string parent);

        EditorResponse SendKeys(string value, string root, string name, string parent);

        EditorResponse Exists(string root, string name, string parent);

        EditorResponse Active(string root, string name, string parent);

        EditorResponse StartPlayMode();

        EditorResponse StopPlayMode();

        EditorResponse MakeScreenshot(string fullPath);
    }
}
