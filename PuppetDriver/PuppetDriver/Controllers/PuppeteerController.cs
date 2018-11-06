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
            string upath = null;
            string value = null;

            var request = new Dictionary<string, string>();
            var body = new StreamReader(context.Request.Body).ReadToEnd();
            try
            {
                request = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var response = new Dictionary<string, string> {{Parameters.ErrorMessage, string.Empty}};

            if (!request.ContainsKey(Parameters.Method))
            {
                response.Add(Parameters.StatusCode, ErrorCodes.MethodNodeIsNotPresent.ToString());
                response[Parameters.ErrorMessage] = "Node method is missed";
                response.Add(Parameters.Result, ActionResults.Fail);
                return context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented));
            }
            if (request.ContainsKey(Parameters.Name)) gameObjectName = request[Parameters.Name];
            if (request.ContainsKey(Parameters.Root)) gameObjectRootName = request[Parameters.Root];
            if (request.ContainsKey(Parameters.Parent)) gameObjectParentName = request[Parameters.Parent];
            if (request.ContainsKey(Parameters.UPath)) upath = request[Parameters.UPath];
            if (request.ContainsKey(Parameters.Value)) value = request[Parameters.Value];

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
                    result = handler.Click(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.SendKeys:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.SendKeys(value, gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.Exist:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.Exists(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.Active:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.Active(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.Swipe:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.Swipe(gameObjectRootName, gameObjectName, gameObjectParentName, upath, value);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                
                case Methods.DragTo:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.DragTo(gameObjectRootName, gameObjectName, gameObjectParentName, upath, value);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;

                case Methods.Rendering:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.Rendering(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                
                case Methods.OnScreen:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.OnScreen(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                
                case Methods.GraphicClickable:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.GraphicClickable(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                
                case Methods.Count:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.Count(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                
                case Methods.GetComponent:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.GetComponent(gameObjectRootName, gameObjectName, gameObjectParentName, upath, value);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                
                case Methods.GetCoordinates:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);

                    result = handler.GetCoordinates(gameObjectRootName, gameObjectName, gameObjectParentName, upath);
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
                    ConnectionManager.ReleaseEditorHandler(sessionId);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;
                case Methods.KillAllSessions:
                    ConnectionManager.ReleaseAllEditorHandlers();
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;
                
                case Methods.DeletePlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    result = handler.DeletePlayerPref(value);
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
                    break;
                case Methods.DeleteAllPrefs:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetEditorHandler(sessionId);
                    result = handler.DeleteAllPrefs();
                    response.Add(Parameters.StatusCode, result.StatusCode.ToString());
                    response.Add(Parameters.Result, result.Result);
                    response[Parameters.ErrorMessage] = result.ErrorMessage;
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
