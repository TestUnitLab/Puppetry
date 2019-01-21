using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

using Puppetry.Contracts;
using Puppetry.Puppeteer.Exceptions;

namespace Puppetry.Puppeteer.Driver
{
    internal class PuppetDriverClient
    {
        private string _sessionId;

        public PuppetDriverClient()
        {
            StartSession();
        }
        
        internal bool IsPlayMode()
        {
            var request = BuildRequest(Methods.IsPlayMode, _sessionId);
            var response = Post(request);
  
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                return false;

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal void Click(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.Click, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"GameObject with {locatorMessage} was not clicked");
        }

        internal void SendKeys(string value, string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.SendKeys, _sessionId, upath: upath, root: root, name: name, parent: parent, value: value);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"Keys {value} were not sent to GameObject with {locatorMessage}");
        }

        internal void Swipe(string root, string name, string parent, string upath, string direction, string locatorMessage)
        {
            var request = BuildRequest(Methods.Swipe, _sessionId, upath: upath, root: root, name: name, parent: parent, value: direction);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"GameObject with {locatorMessage} was not clicked");
        }
        
        internal void DragTo(string root, string name, string parent, string upath, string toCoordinates, string locatorMessage)
        {
            var request = BuildRequest(Methods.DragTo, _sessionId, upath: upath, root: root, name: name, parent: parent, value: toCoordinates);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"GameObject with {locatorMessage} was not clicked");
        }

        internal bool Exist(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Exist, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool Active(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.Active, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool IsRendering(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.Rendering, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool IsOnScreen(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.OnScreen, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }
        
        internal bool IsHitByPhysicsRaycast(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.PhysicClickable, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool IsHitByGraphicRaycast(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.GraphicClickable, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal int Count(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Count, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return int.TryParse(response[Parameters.Result], out var result) ? result : -1;
        }

        internal string GetComponent(string root, string name, string parent, string upath, string component, string locatorMessage)
        {
            var request = BuildRequest(Methods.GetComponent, _sessionId, upath: upath, root: root, name: name, parent: parent, value: component);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return response[Parameters.Result];
        }
        
        internal string GetCoordinates(string root, string name, string parent, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.GetCoordinates, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return response[Parameters.Result];
        }

        internal void StartPlayMode()
        {
            var request = BuildRequest(Methods.StartPlayMode, _sessionId);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.MethodNotSupported.ToString())
                throw new MethodIsNotSupportedException("StartPlayMode method is not available");
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"PlayMode was not started with error: {response[Parameters.ErrorMessage]}");
        }

        internal void StopPlayMode()
        {
            var request = BuildRequest(Methods.StopPlayMode, _sessionId);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MethodNotSupported.ToString())
                throw new MethodIsNotSupportedException("StopPlayMode method is not available");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"PlayMode was not stopped with error: {response[Parameters.ErrorMessage]}");
        }

        internal void TakeScreenshot(string path)
        {
            var request = BuildRequest(Methods.TakeScreenshot, _sessionId, value: path);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"Screenshot was not taken with error: {response[Parameters.ErrorMessage]}");
        }

        internal void ReleaseSession()
        {
            var request = BuildRequest(Methods.KillSession, _sessionId);
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("Session was not released");
        }
        
        internal static void ReleaseAllSessions()
        {
            var request = BuildRequest(Methods.KillAllSessions);
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("All Sessions were not released");
        }

        public void DeletePlayerPref(string key)
        {
            var request = BuildRequest(Methods.DeletePlayerPref, _sessionId, key: key);            
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("PlayerPref was not deleted");
        }
        
        public void DeleteAllPlayerPrefs()
        {
            var request = BuildRequest(Methods.DeleteAllPlayerPrefs, _sessionId);            
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("PlayerPrefs were not deleted");
        }

        public string GetFloatPlayerPref(string key)
        {
            var request = BuildRequest(Methods.GetFloatPlayerPref, _sessionId, key: key);
            var response = Post(request);

            return response[Parameters.Result];
        }

        public string GetIntPlayerPref(string key)
        {
            var request = BuildRequest(Methods.GetIntPlayerPref, _sessionId, key: key);
            var response = Post(request);

            return response[Parameters.Result];
        }

        public string GetStringPlayerPref(string key)
        {
            var request = BuildRequest(Methods.GetStringPlayerPref, _sessionId, key: key);
            var response = Post(request);

            return response[Parameters.Result];
        }

        public void SetFloatPlayerPref(string key, string value)
        {
            var request = BuildRequest(Methods.SetFloatPlayerPref, _sessionId, key: key, value: value);
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException(response[Parameters.Result]);
        }

        public void SetIntPlayerPref(string key, string value)
        {
            var request = BuildRequest(Methods.SetIntPlayerPref, _sessionId, key: key, value: value);
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException(response[Parameters.Result]);
        }

        public void SetStringPlayerPref(string key, string value)
        {
            var request = BuildRequest(Methods.SetStringPlayerPref, _sessionId, key: key, value: value);
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException(response[Parameters.Result]);
        }

        public string PlayerPrefHasKey(string key)
        {
            var request = BuildRequest(Methods.PlayerPrefHasKey, _sessionId, key: key);
            var response = Post(request);

            return response[Parameters.Result];
        }
        
        public void ClickAnywhere()
        {
            var request = BuildRequest(Methods.Custom, _sessionId, key: "clickanywhere");
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException(response[Parameters.Result]);
        }

        public void Zoom(string direction)
        {
            var request = BuildRequest(Methods.Custom, _sessionId, key: "zoom", value: direction);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException(response[Parameters.Result]);
        }

        private void StartSession()
        {
            var isSuccess = false;
            var alreadyWaited = 0;
            var timeToWait = Configuration.PollingStratagy == PollingStrategies.Progressive ? 0 : 500;
            var stopwatch = new Stopwatch();

            Dictionary<string, string> response;

            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();
                var result = false;

                var request = BuildRequest(Methods.CreateSession);
                response = Post(request);
                if (response[Parameters.StatusCode] == ErrorCodes.Success.ToString())
                {
                    _sessionId = response[Parameters.Result];
                    result = true;
                }

                if (result)
                {
                    isSuccess = true;
                    break;
                }

                stopwatch.Stop();

                alreadyWaited += stopwatch.Elapsed.Milliseconds;

                if (alreadyWaited >= Configuration.SessionTimeoutMs)
                    break;

                if (Configuration.PollingStratagy == PollingStrategies.Progressive)
                {
                    if (timeToWait == 0) timeToWait += 100;
                    else timeToWait *= 2;
                }

                Thread.Sleep(timeToWait);

                alreadyWaited += timeToWait;
            }

            if (!isSuccess)
                throw new SessionCreationException(
                    $"Session Creation was failed. {response[Parameters.ErrorMessage]}");

        }

        private static Dictionary<string, string> BuildRequest(string method, string session = null, string upath = null,
            string root = null, string name = null, string parent = null, string key = null, string value = null)
        {
            var request = new Dictionary<string, string>();
            
            request.Add(Parameters.Method, method);
            if (!string.IsNullOrEmpty(session)) request.Add(Parameters.Session, session);
            if (!string.IsNullOrEmpty(upath)) request.Add(Parameters.UPath, upath);
            if (!string.IsNullOrEmpty(root)) request.Add(Parameters.Root, root);
            if (!string.IsNullOrEmpty(name)) request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) request.Add(Parameters.Parent, parent);
            if (!string.IsNullOrEmpty(key)) request.Add(Parameters.Key, key);
            if (!string.IsNullOrEmpty(value)) request.Add(Parameters.Value, value);

            return request;
        }

        private static Dictionary<string, string> Post(Dictionary<string, string> request)
        {
            var restClient = new RestClient($"{Configuration.BaseUrl}:{Configuration.Port}/");
            restClient.Timeout = 10000;
            var restRequest = new RestRequest(Method.POST);
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);
            restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
            
            var restResponse = restClient.Execute(restRequest);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
            {
                return new Dictionary<string, string>
                {
                    {Parameters.StatusCode,  ErrorCodes.PuppetDriverError.ToString()},
                    {Parameters.ErrorMessage,  restResponse.ErrorMessage},
                    {Parameters.Result,  ActionResults.Fail}
                };
            }
            
            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);

            return response;
        }

        private static void ValidateResponseStructure(Dictionary<string, string> response)
        {
            if (!response.ContainsKey(Parameters.StatusCode) || !response.ContainsKey(Parameters.Result))
                throw new UnexpectedResponseException($"PuppetDriver sent unexpected response: {response}");
        }
    }
}