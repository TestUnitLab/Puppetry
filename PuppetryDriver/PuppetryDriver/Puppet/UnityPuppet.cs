using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Puppetry.Contracts;
using Puppetry.PuppetryDriver.TcpSocket;

namespace Puppetry.PuppetryDriver.Puppet
{
    internal class UnityPuppet : IPuppetHandler
    {
        private const string GameObjectNotFoundMessage = "GameObject was not found";
        private const string ComponentNotFoundMessage = "Component was not found";
        private const string MainThreadIsUnavailable = "Main Thread is unavailable or overloaded";
        private const string MethodIsNotSupported = "Method is not supported";
        private const string Error = "Error";

        public string Session { get; }
        public Socket Socket { get; set; }
        public bool Available { get; set; }
        public DateTime LastPing { get; set; }

        public UnityPuppet(Socket socket, string session)
        {
            Session = session;
            Socket = socket;
            Available = true;
            LastPing = DateTime.UtcNow;
        }
        
        public PuppetResponse IsPlayMode()
        {
            return ProcessMethod(Methods.IsPlayMode);
        }

        public PuppetResponse SendKeys(string value, string upath)
        {
            return ProcessMethod(Methods.SendKeys, upath: upath, value: value);
        }       

        public PuppetResponse Click(string upath)
        {
            return ProcessMethod(Methods.Click, upath: upath);
        }

        public PuppetResponse Exists(string upath)
        {
            return ProcessMethod(Methods.Exist, upath: upath);
        }
        
        public PuppetResponse Rendering(string upath)
        {
            return ProcessMethod(Methods.Rendering, upath: upath);
        }
        
        public PuppetResponse OnScreen(string upath)
        {
            return ProcessMethod(Methods.OnScreen, upath: upath);
        }
        
        public PuppetResponse GraphicClickable(string upath)
        {
            return ProcessMethod(Methods.GraphicClickable, upath: upath);
        }
        
        public PuppetResponse PhysicClickable(string upath)
        {
            return ProcessMethod(Methods.PhysicClickable, upath: upath);
        }
        
        public PuppetResponse GetComponent(string upath, string component)
        {
            return ProcessMethod(Methods.GetComponent, upath: upath, value: component);
        }
        
        public PuppetResponse GetCoordinates(string upath)
        {
            return ProcessMethod(Methods.GetCoordinates, upath: upath);
        }
        
        public PuppetResponse Count(string upath)
        {
            return ProcessMethod(Methods.Count, upath: upath);
        }

        public PuppetResponse Active(string upath)
        {
            return ProcessMethod(Methods.Active, upath: upath);
        }

        public PuppetResponse Swipe(string upath, string direction)
        {
            return ProcessMethod(Methods.Swipe, upath: upath, value: direction);
        }
        
        public PuppetResponse DragTo(string upath, string toCoordinates)
        {
            return ProcessMethod(Methods.Swipe, upath: upath, value: toCoordinates);
        }

        public PuppetResponse StartPlayMode()
        {
            return ProcessMethod(Methods.StartPlayMode);
        }

        public PuppetResponse StopPlayMode()
        {
            return ProcessMethod(Methods.StopPlayMode);
        }

        public PuppetResponse MakeScreenshot(string fullPath)
        {
            return ProcessMethod(Methods.TakeScreenshot, value: fullPath);
        }
        
        public PuppetResponse DeletePlayerPref(string key)
        {
            return ProcessMethod(Methods.DeletePlayerPref, key: key);
        }
        
        public PuppetResponse DeleteAllPlayerPrefs()
        {
            return ProcessMethod(Methods.DeleteAllPlayerPrefs);
        }

        public PuppetResponse GetFloatPlayerPref(string key)
        {
            return ProcessMethod(Methods.GetFloatPlayerPref, key: key);
        }

        public PuppetResponse GetIntPlayerPref(string key)
        {
            return ProcessMethod(Methods.GetIntPlayerPref, key: key);
        }

        public PuppetResponse GetStringPlayerPref(string key)
        {
            return ProcessMethod(Methods.GetStringPlayerPref, key: key);
        }

        public PuppetResponse SetFloatPlayerPref(string key, string value)
        {
            return ProcessMethod(Methods.SetFloatPlayerPref, key: key, value: value);
        }

        public PuppetResponse SetIntPlayerPref(string key, string value)
        {
            return ProcessMethod(Methods.SetIntPlayerPref, key: key, value: value);
        }

        public PuppetResponse SetStringPlayerPref(string key, string value)
        {
            return ProcessMethod(Methods.SetStringPlayerPref, key: key, value: value);
        }

        public PuppetResponse PlayerPrefHasKey(string key)
        {
            return ProcessMethod(Methods.PlayerPrefHasKey, key: key);
        }
        
        public PuppetResponse GameCustom(string key, string value)
        {
            return ProcessMethod(Methods.GameCustom, key: key, value: value);
        }

        public PuppetResponse GameObjectCustom(string upath, string key, string value)
        {
            return ProcessMethod(Methods.GameObjectCustom, upath: upath, key: key, value: value);
        }

        public PuppetResponse GetSceneName()
        {
            return ProcessMethod(Methods.GetScene);
        }

        public PuppetResponse OpenScene(string key)
        {
            return ProcessMethod(Methods.OpenScene, key: key);
        }

        public PuppetResponse GetSpriteName(string upath)
        {
            return ProcessMethod(Methods.GetSpriteName, upath: upath);
        }

        private PuppetResponse ProcessMethod(string method, string upath = null, string key = null, string value = null)
        {
            var request = PrepareRequest(method, upath, key, value);

            var response = SocketHelper.SendMessage(Socket, request);
            PuppetResponse result = null;
            if (response == null)
                result = new PuppetResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!response.ContainsKey(Parameters.Method) || response[Parameters.Method] != method)
                result = new PuppetResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else if (response[Parameters.Result] == MainThreadIsUnavailable)
                result = new PuppetResponse { StatusCode = ErrorCodes.MainThreadIsUnavailable, IsSuccess = false, ErrorMessage = response[Parameters.Result] };
            else if (response.ContainsKey(Error))
                result = new PuppetResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = response[Parameters.Result] };
            else if (response[Parameters.Result] == MethodIsNotSupported)
                result = new PuppetResponse { StatusCode = ErrorCodes.MethodNotSupported, IsSuccess = false, ErrorMessage = response[Parameters.Result] };
            else if (response[Parameters.Result] == GameObjectNotFoundMessage)
                result = new PuppetResponse { StatusCode = ErrorCodes.NoSuchGameObjectFound, IsSuccess = false, ErrorMessage = response[Parameters.Result] };
            else if (response[Parameters.Result] == ComponentNotFoundMessage)
                result = new PuppetResponse { StatusCode = ErrorCodes.NoSuchComponentFound, IsSuccess = false, ErrorMessage = response[Parameters.Result] };
            else
                result = new PuppetResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = response[Parameters.Result] };

            return result;
        }

        private Dictionary<string, string> PrepareRequest(string method, string upath = null, string key = null, string value = null)
        {
            var request = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(method)) request.Add(Parameters.Method, method);
            if (!string.IsNullOrEmpty(key)) request.Add(Parameters.Key, key);
            if (!string.IsNullOrEmpty(value)) request.Add(Parameters.Value, value);
            if (!string.IsNullOrEmpty(upath)) request.Add(Parameters.UPath, upath);

            return request;
        }
    }
}
