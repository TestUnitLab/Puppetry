using System;
using System.Net.Sockets;

namespace Puppetry.PuppetDriver.Puppet
{
    interface IPuppetHandler
    {
        string Session { get;}

        bool Available { get; set; }

        DateTime LastPing { get; set; }

        Socket Socket { get; set; }
        
        PuppetResponse IsPlayMode();

        PuppetResponse Click(string root, string name, string parent, string upath);

        PuppetResponse SendKeys(string value, string root, string name, string parent, string upath);

        PuppetResponse Exists(string root, string name, string parent, string upath);

        PuppetResponse Active(string root, string name, string parent, string upath);

        PuppetResponse Swipe(string root, string name, string parent, string upath, string direction);
        
        PuppetResponse DragTo(string root, string name, string parent, string upath, string toCoordinates);

        PuppetResponse Rendering(string root, string name, string parent, string upath);
        
        PuppetResponse OnScreen(string root, string name, string parent, string upath);
        
        PuppetResponse GraphicClickable(string root, string name, string parent, string upath);
        
        PuppetResponse Count(string root, string name, string parent, string upath);
        
        PuppetResponse GetComponent(string root, string name, string parent, string upath, string component);
        
        PuppetResponse GetCoordinates(string root, string name, string parent, string upath);

        PuppetResponse StartPlayMode();

        PuppetResponse StopPlayMode();

        PuppetResponse MakeScreenshot(string fullPath);
        
        PuppetResponse DeletePlayerPref(string key);
        
        PuppetResponse DeleteAllPlayerPrefs();

        PuppetResponse GetFloatPlayerPref(string key);

        PuppetResponse GetIntPlayerPref(string key);

        PuppetResponse GetStringPlayerPref(string key);

        PuppetResponse SetFloatPlayerPref(string key, string value);

        PuppetResponse SetIntPlayerPref(string key, string value);

        PuppetResponse SetStringPlayerPref(string key, string value);

        PuppetResponse PlayerPrefHasKey(string key);
    }
}
