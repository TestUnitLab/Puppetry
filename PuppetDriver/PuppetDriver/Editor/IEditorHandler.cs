
namespace PuppetDriver.Editor
{
    interface IEditorHandler
    {
        string Identificator { get;}

        EditorResponse Click(string name, string parent);

        EditorResponse SendKeys(string value, string name, string parent);

        EditorResponse Exists(string name, string parent);

        EditorResponse Active(string name, string parent);

        EditorResponse StartPlayMode();

        EditorResponse StopPlayMode();

        EditorResponse MakeScreenshot(string fullPath);
    }
}
