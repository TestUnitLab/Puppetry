using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Puppetry.PuppetContracts;
using Puppetry.PuppetDriver.TcpSocket;

namespace Puppetry.PuppetDriver.Editor
{
    internal class UnityEditor : IEditorHandler
    {
        private Dictionary<string, string> _request;
        private Dictionary<string, string> _response;

        public string Session { get; }
        public Socket Socket { get; set; }

        public UnityEditor(Socket socket, string session)
        {
            Session = session;
            Socket = socket;
            _request = new Dictionary<string, string>();
        }

        public EditorResponse SendKeys(string value, string root, string name, string parent, string upath)
        {
            PrepareRequest(Methods.SendKeys, upath: upath, root: root, name: name, parent: parent, value: value);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.SendKeys)
                result = new EditorResponse {StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }       

        public EditorResponse Click(string root, string name, string parent, string upath)
        {
            PrepareRequest(Methods.Click, upath: upath, root: root, name: name, parent: parent);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Click)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        public EditorResponse Exists(string root, string name, string parent, string upath)
        {
            PrepareRequest(Methods.Exist, upath: upath, root: root, name: name, parent: parent);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Exist)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }
        
        public EditorResponse Rendering(string root, string name, string parent, string upath)
        {
            PrepareRequest(Methods.Rendering, upath: upath, root: root, name: name, parent: parent);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Exist)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }
        
        public EditorResponse GetComponent(string root, string name, string parent, string upath, string component)
        {
            PrepareRequest(Methods.GetComponent, upath: upath, root: root, name: name, parent: parent, value: component);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Exist)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }
        
        public EditorResponse Count(string root, string name, string parent, string upath)
        {
            PrepareRequest(Methods.Count, upath: upath, root: root, name: name, parent: parent);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Exist)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        public EditorResponse Active(string root, string name, string parent, string upath)
        {
            PrepareRequest(Methods.Exist, upath: upath, root: root, name: name, parent: parent);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Active)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        public EditorResponse Swipe(string root, string name, string parent, string upath, string direction)
        {
            PrepareRequest(Methods.Swipe, upath: upath, root: root, name: name, parent: parent, value: direction);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.Active)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        public EditorResponse StartPlayMode()
        {
            PrepareRequest(Methods.StartPlayMode);

            _response = SocketHelper.SendMessage(Socket, _request);
            

            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.StartPlayMode)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        public EditorResponse StopPlayMode()
        {
            PrepareRequest(Methods.StopPlayMode);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.StopPlayMode)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        public EditorResponse MakeScreenshot(string fullPath)
        {
            PrepareRequest(Methods.TakeScreenshot, value: fullPath);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.TakeScreenshot)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }
        
        public EditorResponse DeletePlayerPref(string key)
        {
            PrepareRequest(Methods.DeletePlayerPref, value: key);

            _response = SocketHelper.SendMessage(Socket, _request);
            EditorResponse result;
            if (_response == null)
                result = new EditorResponse { StatusCode = ErrorCodes.PuppetDriverError, IsSuccess = false, ErrorMessage = "Communication Error exception in PuppetDriver" };
            else if (!_response.ContainsKey(Parameters.Method) && _response[Parameters.Method] != Methods.TakeScreenshot)
                result = new EditorResponse { StatusCode = ErrorCodes.UnexpectedResponse, IsSuccess = false, ErrorMessage = "Unexpected request was received" };
            else
                result = new EditorResponse { StatusCode = ErrorCodes.Success, IsSuccess = true, Result = _response[Parameters.Result] };

            return result;
        }

        private void PrepareRequest(string method, string upath = null, string root = null, string name = null, string parent = null, string value = null)
        {
            _request.Clear();
            if (!string.IsNullOrEmpty(method)) _request.Add(Parameters.Method, method);
            if (!string.IsNullOrEmpty(value)) _request.Add(Parameters.Value, value);
            if (!string.IsNullOrEmpty(root)) _request.Add(Parameters.Root, root);
            if (!string.IsNullOrEmpty(name)) _request.Add(Parameters.Name, name);
            if (!string.IsNullOrEmpty(parent)) _request.Add(Parameters.Parent, parent);
            if (!string.IsNullOrEmpty(upath)) _request.Add(Parameters.UPath, upath);
        }
    }
}
