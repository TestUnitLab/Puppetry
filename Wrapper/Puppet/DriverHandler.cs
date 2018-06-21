using System;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.IO;

namespace Puppet
{
    internal class DriverHandler : IDisposable
    {
        private string _sessionId;

        public DriverHandler()
        {
            StartSession();
        }

        internal void Click(string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "click");
            request.Add("session", _sessionId);
            request.Add("name", name);
            if (!string.IsNullOrEmpty(parent)) request.Add("parent", parent);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            if (json["result"] != "success")
                throw new Exception($"GameObject with name: {name} and parent: {parent ?? "null"} was not clicked");
        }

        internal void SendKeys(string value, string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "sendkeys");
            request.Add("session", _sessionId);
            request.Add("value", value);
            request.Add("name", name);
            if (!string.IsNullOrEmpty(parent)) request.Add("parent", parent);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            if (json["result"] != "success")
                throw new Exception($"Keys {value} were not sent to GameObject with name: {name} and parent: {parent ?? "null"}");
        }

        internal bool Exist(string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "exist");
            request.Add("session", _sessionId);
            request.Add("name", name);
            if (!string.IsNullOrEmpty(parent)) request.Add("parent", parent);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());

            bool result;
            return (bool.TryParse(json["result"], out result) && result);
        }

        internal bool Active(string name, string parent = null)
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "active");
            request.Add("session", _sessionId);
            request.Add("name", name);
            if (!string.IsNullOrEmpty(parent)) request.Add("parent", parent);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());

            bool result;
            return (bool.TryParse(json["result"], out result) && result);
        }

        internal void StartPlayMode()
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "startplaymode");
            request.Add("session", _sessionId);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            if (json["result"] != "success")
                throw new Exception($"PlayMode was not started");
        }

        internal void StopPlayMode()
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "stopplaymode");
            request.Add("session", _sessionId);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            if (json["result"] != "success")
                throw new Exception($"PlayMode was not stopped");
        }

        internal void TakeScreenshot(string path)
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "takescreenshot");
            request.Add("session", _sessionId);
            request.Add("path", path);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            if (json["result"] != "success")
                throw new Exception($"PlayMode was not stopped");
        }

        private void StartSession()
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "createsession");

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            _sessionId = json["result"];
        }

        private void KillSession()
        {
            var request = new Dictionary<string, string>();
            request.Add("method", "killsession");
            request.Add("session", _sessionId);

            var response = Post(request);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);

            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(response.Content).ReadToEnd());
            _sessionId = json["result"];
        }

        private IRestResponse Post(Dictionary<string, string> request)
        {
            var restClient = new RestClient($"http://localhost:7111/");
            var restRequest = new RestRequest(Method.POST);

            restRequest.AddJsonBody(JsonConvert.SerializeObject(request, Formatting.Indented));
            return restClient.Execute(restRequest);
        }

        public void Dispose()
        {
            KillSession();
        }
    }
}
