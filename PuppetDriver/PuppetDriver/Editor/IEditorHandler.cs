
using System.Net.Sockets;

namespace Puppetry.PuppetDriver.Editor
{
    interface IEditorHandler
    {
        string Session { get;}

        Socket Socket { get; set; }

        EditorResponse Click(string root, string name, string parent, string upath);

        EditorResponse SendKeys(string value, string root, string name, string parent, string upath);

        EditorResponse Exists(string root, string name, string parent, string upath);

        EditorResponse Active(string root, string name, string parent, string upath);

        EditorResponse Swipe(string root, string name, string parent, string upath, string direction);

        EditorResponse Rendering(string root, string name, string parent, string upath);
        
        EditorResponse OnScreen(string root, string name, string parent, string upath);
        
        EditorResponse GraphicClickable(string root, string name, string parent, string upath);
        
        EditorResponse Count(string root, string name, string parent, string upath);
        
        EditorResponse GetComponent(string root, string name, string parent, string upath, string component);

        EditorResponse StartPlayMode();

        EditorResponse StopPlayMode();

        EditorResponse MakeScreenshot(string fullPath);
        
        EditorResponse DeletePlayerPref(string key);
    }
}
