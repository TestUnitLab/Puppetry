using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

using PuppetDriver.Editor;
using static DriverContracts.Contracts;

namespace PuppetDriver.Controllers
{
    internal class PuppeteerController
    {
        public Task ProcessRequest(HttpContext context)
        { 
            string sessionId;
            IEditorHandler handler;
            string gameObjectName = null;
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
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, sessionId);
                    break;

                case Methods.Click:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request[Parameters.Name];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    handler.Click(gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;

                case Methods.SendKeys:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var value = request[Parameters.Value];
                    gameObjectName = request[Parameters.Name];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    handler.SendKeys(value, gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;

                case Methods.Exist:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request[Parameters.Name];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    var isExisted = handler.Exists(gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, isExisted.ToString().ToLowerInvariant());
                    break;

                case Methods.Active:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    gameObjectName = request[Parameters.Name];
                    if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];

                    var isActive = handler.Active(gameObjectName, gameObjectParentName);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, isActive.ToString().ToLowerInvariant());
                    break;

                case Methods.StartPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    handler.StartPlayMode();
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;

                case Methods.StopPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    handler.StopPlayMode();
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;

                case Methods.TakeScreenshot:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    var fullPath = request[Parameters.Path];

                    handler.MakeScreenshot(fullPath);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
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
