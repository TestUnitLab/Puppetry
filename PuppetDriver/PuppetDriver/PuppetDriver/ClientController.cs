using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PuppetDriver.Editor;

namespace PuppetDriver
{
    public class ClientController
    {
        public Task ProcessRequest(HttpContext context)
        { 
            string sessionId;
            IEditorHandler handler;
            string gameObjectName = null;
            string gameObjectParentName = null;

            var request = JsonConvert.DeserializeObject<Dictionary<string, string>>(new StreamReader(context.Request.Body).ReadToEnd());

            var response = new Dictionary<string, string>();
            response.Add("errormessage", string.Empty);

            if (!request.ContainsKey("method"))
            {
                response.Add("statuscode", "11");
                response["errormessage"] = "Node method is missed";
                response.Add("result", "fail");
                return context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
            }

            switch (request["method"].ToLowerInvariant())
            {
                case "createsession":
                    sessionId = ConnectionManager.StartSession();
                    response.Add("statuscode", "0");
                    response.Add("result", sessionId);
                    break;

                case "click":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    handler.Click(gameObjectName, gameObjectParentName);
                    response.Add("statuscode", "0");
                    response.Add("result", "success");
                    break;

                case "sendkeys":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var value = request["value"];
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    handler.SendKeys(value, gameObjectName, gameObjectParentName);
                    response.Add("statuscode", "0");
                    response.Add("result", "success");
                    break;

                case "exist":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    var isExisted = handler.Exists(gameObjectName, gameObjectParentName);
                    response.Add("statuscode", "0");
                    response.Add("result", isExisted.ToString().ToLowerInvariant());
                    break;

                case "active":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    var isActive = handler.Active(gameObjectName, gameObjectParentName);
                    response.Add("statuscode", "0");
                    response.Add("result", isActive.ToString().ToLowerInvariant());
                    break;

                case "startplaymode":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    handler.StartPlayMode();
                    response.Add("statuscode", "0");
                    response.Add("result", "success");
                    break;

                case "stopplaymode":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    handler.StopPlayMode();
                    response.Add("statuscode", "0");
                    response.Add("result", "success");
                    break;

                case "takescreenshot":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var fullPath = request["path"];

                    handler.MakeScreenshot(fullPath);
                    response.Add("statuscode", "0");
                    response.Add("result", "success");
                    break;

                case "killsession":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    ConnectionManager.ReleaseEditorHandler(sessionId);
                    response.Add("statuscode", "0");
                    response.Add("result", "success");
                    break;

                default:
                    response.Add("statuscode", "5");
                    response["errormessage"] = $"Method: '{request["method"]}' is not suppoerted";
                    response.Add("result", "fail");
                    break;
            }

            var json = JsonConvert.SerializeObject(response, Formatting.Indented);
            return context.Response.WriteAsync(json);
        }
    }
}
