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

        internal void Click(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.Click, _sessionId, upath: upath);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"GameObject with {locatorMessage} was not clicked");
        }

        internal void SendKeys(string value, string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.SendKeys, _sessionId, upath: upath, value: value);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"Keys {value} were not sent to GameObject with {locatorMessage}");
        }

        internal void Swipe(string upath, string direction, string locatorMessage)
        {
            var request = BuildRequest(Methods.Swipe, _sessionId, upath: upath, value: direction);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"GameObject with {locatorMessage} was not clicked");
        }
        
        internal void DragTo(string upath, string toCoordinates, string locatorMessage)
        {
            var request = BuildRequest(Methods.DragTo, _sessionId, upath: upath, value: toCoordinates);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"GameObject with {locatorMessage} was not clicked");
        }

        internal bool Exist(string upath)
        {
            var request = BuildRequest(Methods.Exist, _sessionId, upath: upath);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool Active(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.Active, _sessionId, upath: upath);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool IsRendering(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.Rendering, _sessionId, upath: upath);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool IsOnScreen(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.OnScreen, _sessionId, upath: upath);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }
        
        internal bool IsHitByRaycast(string upath, string raycaster, string locatorMessage)
        {
            var request = BuildRequest(Methods.Raycasted, _sessionId, upath: upath, value: raycaster);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal int Count(string upath)
        {
            var request = BuildRequest(Methods.Count, _sessionId, upath: upath);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return int.TryParse(response[Parameters.Result], out var result) ? result : -1;
        }

        internal string GetComponent(string upath, string component, string locatorMessage)
        {
            var request = BuildRequest(Methods.GetComponent, _sessionId, upath: upath, value: component);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return response[Parameters.Result];
        }
        
        internal string GetCoordinates(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.GetCoordinates, _sessionId, upath: upath);
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
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException($"Screenshot was not taken with error: {response[Parameters.ErrorMessage]}");
        }

        internal void ReleaseSession()
        {
            var request = BuildRequest(Methods.KillSession, _sessionId);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("Session was not released");
        }
        
        internal static void ReleaseAllSessions()
        {
            var request = BuildRequest(Methods.KillAllSessions);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("All Sessions were not released");
        }

        public void DeletePlayerPref(string key)
        {
            var request = BuildRequest(Methods.DeletePlayerPref, _sessionId, key: key);            
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("PlayerPref was not deleted");
        }
        
        public void DeleteAllPlayerPrefs()
        {
            var request = BuildRequest(Methods.DeleteAllPlayerPrefs, _sessionId);            
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException("PlayerPrefs were not deleted");
        }

        public string GetFloatPlayerPref(string key)
        {
            var request = BuildRequest(Methods.GetFloatPlayerPref, _sessionId, key: key);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);

            return response[Parameters.Result];
        }

        public string GetIntPlayerPref(string key)
        {
            var request = BuildRequest(Methods.GetIntPlayerPref, _sessionId, key: key);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);

            return response[Parameters.Result];
        }

        public string GetStringPlayerPref(string key)
        {
            var request = BuildRequest(Methods.GetStringPlayerPref, _sessionId, key: key);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);

            return response[Parameters.Result];
        }

        public void SetFloatPlayerPref(string key, string value)
        {
            var request = BuildRequest(Methods.SetFloatPlayerPref, _sessionId, key: key, value: value);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
        }

        public void SetIntPlayerPref(string key, string value)
        {
            var request = BuildRequest(Methods.SetIntPlayerPref, _sessionId, key: key, value: value);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
        }

        public void SetStringPlayerPref(string key, string value)
        {
            var request = BuildRequest(Methods.SetStringPlayerPref, _sessionId, key: key, value: value);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
        }

        public string PlayerPrefHasKey(string key)
        {
            var request = BuildRequest(Methods.PlayerPrefHasKey, _sessionId, key: key);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);

            return response[Parameters.Result];
        }

        public string GetSceneName()
        {
            var request = BuildRequest(Methods.GetScene, _sessionId);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);

            return response[Parameters.Result];
        }

        public void OpenScene(string sceneName)
        {
            var request = BuildRequest(Methods.OpenScene, _sessionId, key: sceneName);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new PuppetryException(response[Parameters.Result]);
        }

        public void GameCustomMethod(string method, string value)
        {
            var request = BuildRequest(Methods.GameCustom, _sessionId, key: method, value: value);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.Result] != ActionResults.Success)
                throw new PuppetryException(response[Parameters.Result]);
        }

        internal string GameObjectCustomMethod(string upath, string method, string value, string locatorMessage)
        {
            var request = BuildRequest(Methods.GameObjectCustom, _sessionId, upath: upath, key: method, value: value);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return response[Parameters.Result]; ;
        }

        internal string GetSpriteName(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.GetSpriteName, _sessionId, upath: upath);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchComponentFound.ToString())
                throw new NoSuchComponentException($"GameObject with {locatorMessage} doesn't have component with sprite property");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return response[Parameters.Result]; ;
        }

        internal string GetGameObjectInfo(string upath, string locatorMessage)
        {
            var request = BuildRequest(Methods.GetGameObjectInfo, _sessionId, upath: upath);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with {locatorMessage} was not found");
            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();

            return response[Parameters.Result]; ;
        }

        public void SetTimeScale(float timeScale)
        {
            var request = BuildRequest(Methods.SetTimeScale, _sessionId, value: timeScale.ToString());
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.MainThreadIsUnavailable.ToString())
                throw new MainThreadUnavailableException();
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
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

        private static Dictionary<string, string> BuildRequest(string method, string session = null, string upath = null, string key = null, string value = null)
        {
            var request = new Dictionary<string, string>();
            
            request.Add(Parameters.Method, method);
            if (!string.IsNullOrEmpty(session)) request.Add(Parameters.Session, session);
            if (!string.IsNullOrEmpty(upath)) request.Add(Parameters.UPath, upath);
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