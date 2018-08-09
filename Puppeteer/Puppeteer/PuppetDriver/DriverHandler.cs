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

        internal void Click(string root, string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.Click);
            request.Add(Parameters.Session, _sessionId);
            request.Add(Parameters.Root, root);
            request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) request.Add(Parameters.Parent, parent);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"GameObject with name: {name} and parent: {parent ?? "null"} was not clicked");
        }

        internal void SendKeys(string value, string root, string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.SendKeys);
            request.Add(Parameters.Session, _sessionId);
            request.Add(Parameters.Value, value);
            request.Add(Parameters.Root, root);
            request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) request.Add(Parameters.Parent, parent);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"Keys {value} were not sent to GameObject with name: {name} and parent: {parent ?? "null"}");
        }

        internal bool Exist(string root, string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.Exist);
            request.Add(Parameters.Session, _sessionId);
            request.Add(Parameters.Root, root);
            request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) request.Add(Parameters.Parent, parent);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);

            bool result;
            return (bool.TryParse(response[Parameters.Result], out result) && result);
        }

        internal bool Active(string root, string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.Active);
            request.Add(Parameters.Session, _sessionId);
            request.Add(Parameters.Root, root);
            request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) request.Add(Parameters.Parent, parent);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.StatusCode] == ErrorCodes.NoSuchGameObjectFound.ToString())
                throw new NoSuchGameObjectException($"GameObject with name: {name} and parent: {parent ?? "null"} was not found");

            bool result;
            return (bool.TryParse(response[Parameters.Result], out result) && result);
        }

        internal void StartPlayMode()
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.StartPlayMode);
            request.Add(Parameters.Session, _sessionId);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"PlayMode was not started");
        }

        internal void StopPlayMode()
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.StopPlayMode);
            request.Add(Parameters.Session, _sessionId);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"PlayMode was not stopped");
        }

        internal void TakeScreenshot(string path)
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.TakeScreenshot);
            request.Add(Parameters.Session, _sessionId);
            request.Add(Parameters.Value, path);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception($"PlayMode was not stopped");
        }

        internal void KillSession()
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.KillSession);
            request.Add(Parameters.Session, _sessionId);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);

            if (response[Parameters.Result] != ActionResults.Success)
                throw new Exception("Session was not Killed");
        }

        private void StartSession()
        {
            var request = new Dictionary<string, string>();
            request.Add(Parameters.Method, Methods.CreateSession);

            var restResponse = Post(request);
            if (!string.IsNullOrEmpty(restResponse.ErrorMessage))
                throw new Exception(restResponse.ErrorMessage);

            var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(restResponse.Content);
            ValidateResponseStructure(response);
            if (response[Parameters.StatusCode] != ErrorCodes.Success.ToString())
                throw new SessionCreationException($"Session Creation was failed. {response[Parameters.ErrorMessage]}");
            _sessionId = response[Parameters.Result];
        }

        public void Dispose()
        {
            KillSession();
        }

        private IRestResponse Post(Dictionary<string, string> request)
        {
            var restClient = new RestClient($"http://localhost:7111/");
            var restRequest = new RestRequest(Method.POST);

            var json = JsonConvert.SerializeObject(request, Formatting.Indented);

            restRequest.AddParameter("application/json", json, ParameterType.RequestBody);
            return restClient.Execute(restRequest);
        }

        private void ValidateResponseStructure(Dictionary<string, string> response)
        {
            if (!response.ContainsKey(Parameters.StatusCode) || !response.ContainsKey(Parameters.Result))
                throw new UnexpectedResponseException($"PuppetDriver sent unexpected response: {response}");
        }
    }
}
