using System;
using System.Net.Sockets;

namespace Puppetry.PuppetryDriver.Puppet
{
    interface IPuppetHandler
    {
        string Session { get;}

        bool Available { get; set; }

        DateTime LastPing { get; set; }

        Socket Socket { get; set; }
        
        PuppetResponse IsPlayMode();

        PuppetResponse Click(string upath);

        PuppetResponse SendKeys(string value, string upath);

        PuppetResponse Exists(string upath);

        PuppetResponse Active(string upath);

        PuppetResponse Swipe(string upath, string direction);
        
        PuppetResponse DragTo(string upath, string toCoordinates);

        PuppetResponse Rendering(string upath);
        
        PuppetResponse OnScreen(string upath);
        
        PuppetResponse GraphicClickable(string upath);
        
        PuppetResponse PhysicClickable(string upath);
        
        PuppetResponse Count(string upath);
        
        PuppetResponse GetComponent(string upath, string component);
        
        PuppetResponse GetCoordinates(string upath);

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

        PuppetResponse GetSceneName();

        PuppetResponse OpenScene(string key);

        PuppetResponse GameCustom(string key, string value);

        PuppetResponse GameObjectCustom(string upath, string key, string value);

        PuppetResponse GetSpriteName(string upath);
    }
}
