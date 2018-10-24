using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using RestSharp;

using Puppetry.PuppetContracts;
using Puppetry.Puppeteer.Exceptions;

namespace Puppetry.Puppeteer.PuppetDriver
{
    internal class DriverHandler : IDisposable
    {
        private string _sessionId;

        public DriverHandler()
        {
            StartSession();
        }

        internal void Click(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Click, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"GameObject with name: {name} and parent: {parent ?? "null"} was not clicked");
        }

        internal void SendKeys(string value, string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.SendKeys, _sessionId, upath: upath, root: root, name: name, parent: parent, value: value);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"Keys {value} were not sent to GameObject with name: {name} and parent: {parent ?? "null"}");
        }

        internal void Swipe(string root, string name, string parent, string upath, string direction)
        {
            var request = BuildRequest(Methods.Click, _sessionId, upath: upath, root: root, name: name, parent: parent, value: direction);
            var response = Post(request);

            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"GameObject with name: {name} and parent: {parent ?? "null"} was not clicked");
        }

        internal bool Exist(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Exist, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }

        internal bool Active(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Active, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }
        
        public bool IsRendering(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Rendering, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }
        
        public bool IsOnScreen(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.OnScreen, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");

            return bool.TryParse(response[Parameters.Result], out var result) && result;
        }
        
        public int Count(string root, string name, string parent, string upath)
        {
            var request = BuildRequest(Methods.Count, _sessionId, upath: upath, root: root, name: name, parent: parent);
            var response = Post(request);

            return int.TryParse(response[Parameters.Result], out var result) ? result : -1;
        }

        public string GetComponent(string root, string name, string parent, string upath, string component)
        {
            var request = BuildRequest(Methods.GetComponent, _sessionId, upath: upath, root: root, name: name, parent: parent, value: component);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");
            
            return response[Parameters.Result];
        }

        internal void StartPlayMode()
        {
            var request = BuildRequest(Methods.StartPlayMode, _sessionId);
            var response = Post(request);
            
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"PlayMode was not started");
        }

        internal void StopPlayMode()
        {
            var request = BuildRequest(Methods.StopPlayMode, _sessionId);
            var response = Post(request);
            
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"PlayMode was not stopped");
        }

        internal void TakeScreenshot(string path)
        {
            var request = BuildRequest(Methods.TakeScreenshot, _sessionId, value: path);
            var response = Post(request);
            
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"PlayMode was not stopped");
        }

        internal void KillSession()
        {
            var request = BuildRequest(Methods.KillSession, _sessionId);
            var response = Post(request);;

            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception("Session was not Killed");
        }

        private void StartSession()
        {
            var request = BuildRequest(Methods.CreateSession);
            var response = Post(request);
            
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new SessionCreationException($"Session Creation was failed. {response[Parameters.ErrorMessage]}");
            _sessionId = response[Parameters.Result];
        }

        public void DeletePlayerPref(string key)
        {
            var request = BuildRequest(Methods.DeletePlayerPref, value: key);            
            var response = Post(request);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception("PlayerPref was not deleted");
        }

        public void Dispose()
        {
            KillSession();
        }

        private Dictionary<string, string> BuildRequest(string method, string session = null, string upath = null,
            string root = null, string name = null, string parent = null, string value = null)
        {
            var request = new Dictionary<string, string>();
            
            request.Add(Parameters.Method, method);
            if (!string.IsNullOrEmpty(session)) request.Add(Parameters.Session, session);
            if (!string.IsNullOrEmpty(upath)) request.Add(Parameters.UPath, upath);
            if (!string.IsNullOrEmpty(root)) request.Add(Parameters.Root, root);
            if (!string.IsNullOrEmpty(name)) request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) request.Add(Parameters.Parent, parent);
            if (!string.IsNullOrEmpty(value)) request.Add(Parameters.Value, value);

            return request;
        }

        private Dictionary<string, string> Post(Dictionary<string, string> request)
        {
            var restClient = new RestClient($"http://localhost:7111/");
            var restRequest = new RestRequest(Method.POST);
            var json = JsonConvert.SerializeObject(request, Formatting.Indented);
            restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
            
            var restResponse = restClient.Execute(restRequest);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);
            
            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);

            return response;
        }

        private void ValidateResponseStructure(Dictionary<string, string> response)
        {
            if (!response.ContainsKey(Parameters.StatusCode) || !response.ContainsKey(Parameters.Result))
                throw new UnexpectedResponseException($"PuppetDriver sent unexpected response: {response}");
        }
    }
}
