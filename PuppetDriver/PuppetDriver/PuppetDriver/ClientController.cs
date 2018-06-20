using System.Collections.Generic;
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

            var request = JsonConvert.DeserializeObject<Dictionary<string, string>>(context.Request.Body.ToString());

            var response = new Dictionary<string, string>();
            response.Add("errorMessage", string.Empty);

            if (!request.ContainsKey("method"))
            {
                response.Add("statusCode", "11");
                response["errorMessage"] = "Node method is missed";
                response.Add("result", "fail");
                return context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
            }

            switch (request["method"])
            {
                case "createSession":
                    sessionId = ConnectionManager.StartSession();
                    response.Add("statusCode", "0");
                    response.Add("result", sessionId);
                    break;

                case "click":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    handler.Click(gameObjectName, gameObjectParentName);
                    response.Add("statusCode", "0");
                    response.Add("result", "success");
                    break;

                case "sendKeys":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    handler.SendKeys(gameObjectName, gameObjectParentName);
                    response.Add("statusCode", "0");
                    response.Add("result", "success");
                    break;

                case "exist":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    var isExisted = handler.Exists(gameObjectName, gameObjectParentName);
                    response.Add("statusCode", "0");
                    response.Add("result", isExisted.ToString().ToLowerInvariant());
                    break;

                case "active":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request["name"];
                    if (request.ContainsKey("parent")) gameObjectParentName = request["parent"];

                    var isActive = handler.Active(gameObjectName, gameObjectParentName);
                    response.Add("statusCode", "0");
                    response.Add("result", isActive.ToString().ToLowerInvariant());
                    break;

                case "startPlayMode":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    handler.StartPlayMode();
                    response.Add("statusCode", "0");
                    response.Add("result", "success");
                    break;

                case "stopPlayMode":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    handler.StopPlayMode();
                    response.Add("statusCode", "0");
                    response.Add("result", "success");
                    break;

                case "takeScreenshot":
                    sessionId = request["session"];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var fullPath = request["path"];

                    handler.MakeScreenshot(fullPath);
                    response.Add("statusCode", "0");
                    response.Add("result", "success");
                    break;
                default:
                    response.Add("statusCode", "5");
                    response["errorMessage"] = $"Method: '{request["method"]}' is not suppoerted";
                    response.Add("result", "fail");
                    break;
            }

            var json = JsonConvert.SerializeObject(response, Formatting.Indented);
            return context.Response.WriteAsync(json);
        }
    }
}
