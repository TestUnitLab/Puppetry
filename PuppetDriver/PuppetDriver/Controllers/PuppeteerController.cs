using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using Puppetry.PuppetDriver.Editor;
using Puppetry.PuppetContracts;
using Puppetry.PuppetDriver;

namespace PuppetDriver.Controllers
{
    internal class PuppeteerController
    {
        public Task ProcessRequest(HttpContext context)
        { 
            string sessionId;
            IEditorHandler handler;
            EditorResponse result;
            string gameObjectName = null;
            string gameObjectRootName = null;
            string gameObjectParentName = null;

            Dictionary<string, string> request = new Dictionary<string, string>();
            var body = new StreamReader(context.Request.Body).ReadToEnd();
            try
            {
                request = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var response = new Dictionary<string, string>();
            response.Add(Parameters.ErrorMessage, string.Empty);

            if (!request.ContainsKey(Parameters.Method))
            {
                response.Add(Parameters.StatusCode, ErrorCodes.MethodNodeIsNotPresent.ToString());
                response[Parameters.ErrorMessage] = "Node method is missed";
                response.Add(Parameters.Result, ActionResults.Fail);
                return context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
            }

            switch (request[Parameters.Method].ToLowerInvariant())
            {
                case Methods.CreateSession:
                    sessionId = ConnectionManager.StartSession();
                    if (sessionId.Contains("Error"))
                    {
                        response.Add(Parameters.StatusCode, ErrorCodes.SessionCreationFailed.ToString());
                        response.Add(Parameters.Result, ActionResults.Fail);
                        response[Parameters.ErrorMessage] = sessionId;
                    }
                    else
                    {
                        response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                        response.Add(Parameters.Result, sessionId);
                    }
                    break;

                case Methods.Click:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request[Parameters.Name];
                    gameObjectRootName = request[Parameters.Root];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    result = handler.Click(gameObjectRootName, gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.SendKeys:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var value = request[Parameters.Value];
                    gameObjectName = request[Parameters.Name];
                    gameObjectRootName = request[Parameters.Root];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    result = handler.SendKeys(value, gameObjectRootName, gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.Exist:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request[Parameters.Name];
                    gameObjectRootName = request[Parameters.Root];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    result = handler.Exists(gameObjectRootName, gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.Active:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request[Parameters.Name];
                    gameObjectRootName = request[Parameters.Root];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    result = handler.Active(gameObjectRootName, gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.StartPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.StartPlayMode();
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.StopPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.StopPlayMode();
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.TakeScreenshot:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var fullPath = request[Parameters.Value];

                    result = handler.MakeScreenshot(fullPath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.KillSession:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    ConnectionManager.ReleaseEditorHandler(sessionId);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;

                default:
                    response.Add(Parameters.StatusCode, ErrorCodes.MethodNotSupported.ToString());
                    response[Parameters.ErrorMessage] = $"Method: '{request["method"]}' is not supported";
                    response.Add(Parameters.Result, ActionResults.Fail);
                    break;
            }

            var json = JsonConvert.SerializeObject(response, Formatting.Indented);
            return context.Response.WriteAsync(json);
        }
    }
}
