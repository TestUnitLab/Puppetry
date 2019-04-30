using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using Puppetry.PuppetryDriver.Puppet;
using Puppetry.Contracts;

namespace Puppetry.PuppetryDriver.Controllers
{
    internal class PuppeteerController
    {
        public Task ProcessRequest(HttpContext context)
        { 
            string sessionId;
            IPuppetHandler handler;
            PuppetResponse result = null;
            string upath = null;
            string key = null;
            string value = null;
            string method = null;

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
            else
            {
                method = request[Parameters.Method];
            }
            if (request.ContainsKey(Parameters.UPath)) upath = request[Parameters.UPath];
            if (request.ContainsKey(Parameters.Key)) key = request[Parameters.Key];
            if (request.ContainsKey(Parameters.Value)) value = request[Parameters.Value];

            Console.WriteLine($"Request => method : {method}, upath : {upath ?? "null"}, key : {key ?? "null"}, value : {value ?? "null"}");

            switch (method.ToLowerInvariant())
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
                
                case Methods.IsPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.IsPlayMode();
                    break;

                case Methods.Click:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.Click(upath);
                    break;

                case Methods.SendKeys:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.SendKeys(value, upath);
                    break;

                case Methods.Exist:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.Exists(upath);
                    break;

                case Methods.Active:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.Active(upath);
                    break;

                case Methods.Swipe:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.Swipe(upath, value);
                    break;
                
                case Methods.DragTo:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.DragTo(upath, value);
                    break;

                case Methods.Rendering:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.Rendering(upath);
                    break;
                
                case Methods.OnScreen:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.OnScreen(upath);
                    break;
                
                case Methods.GraphicClickable:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.GraphicClickable(upath);
                    break;
                
                case Methods.PhysicClickable:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.PhysicClickable(upath);
                    break;
                
                case Methods.Count:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.Count(upath);
                    break;
                
                case Methods.GetComponent:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);

                    result = handler.GetComponent(upath, value);
                    break;
                
                case Methods.GetCoordinates:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GetCoordinates(upath);
                    break;

                case Methods.StartPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.StartPlayMode();
                    break;

                case Methods.StopPlayMode:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.StopPlayMode();
                    break;

                case Methods.TakeScreenshot:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    var fullPath = request[Parameters.Value];
                    result = handler.MakeScreenshot(fullPath);
                    break;

                case Methods.KillSession:
                    sessionId = request[Parameters.Session];
                    ConnectionManager.ReleasePuppetHandler(sessionId);
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;
                case Methods.KillAllSessions:
                    ConnectionManager.ReleaseAllPuppetHandlers();
                    response.Add(Parameters.StatusCode, ErrorCodes.Success.ToString());
                    response.Add(Parameters.Result, ActionResults.Success);
                    break;
                case Methods.DeletePlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.DeletePlayerPref(key);
                    break;
                case Methods.DeleteAllPlayerPrefs:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.DeleteAllPlayerPrefs();
                    break;
                case Methods.GetFloatPlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GetFloatPlayerPref(key);
                    break;
                case Methods.GetIntPlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GetIntPlayerPref(key);
                    break;
                case Methods.GetStringPlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GetStringPlayerPref(key);
                    break;
                case Methods.SetFloatPlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.SetFloatPlayerPref(key, value);
                    break;
                case Methods.SetIntPlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.SetIntPlayerPref(key, value);
                    break;
                case Methods.SetStringPlayerPref:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.SetStringPlayerPref(key, value);
                    break;
                case Methods.PlayerPrefHasKey:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.PlayerPrefHasKey(key);
                    break;
                case Methods.GetScene:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GetSceneName();
                    break;
                case Methods.OpenScene:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.OpenScene(key);
                    break;
                case Methods.GameCustom:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GameCustom(key, value);
                    break;
                case Methods.GameObjectCustom:
                    sessionId = request[Parameters.Session];
                    handler = ConnectionManager.GetPuppetHandler(sessionId);
                    result = handler.GameObjectCustom(upath, key, value);
                    break;

                default:
                    response.Add(Parameters.StatusCode, ErrorCodes.MethodNotSupported.ToString());
                    response[Parameters.ErrorMessage] = $"Method: '{request["method"]}' is not supported";
                    response.Add(Parameters.Result, ActionResults.Fail);
                    break;
            }

            if (!response.ContainsKey(Parameters.StatusCode)) response.Add(Parameters.StatusCode, result.StatusCode.ToString());
            if (!response.ContainsKey(Parameters.StatusCode)) response.Add(Parameters.Result, result.Result);
            if (result != null && !string.IsNullOrEmpty(result.ErrorMessage)) response[Parameters.ErrorMessage] = result.ErrorMessage;

            var json = JsonConvert.SerializeObject(response, Formatting.Indented);
            return context.Response.WriteAsync(json);
        }
    }
}
