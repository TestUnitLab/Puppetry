using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Puppetry.Puppet
{
    public static class DriverHandler
    {
        internal static DriverResponse HandleDriverRequest(DriverRequest request)
        {
            var response = new DriverResponse {method = request.method };
            var session = GetSession();

            switch (request.method.ToLowerInvariant())
            {
                case "registereditor":
                    if (string.IsNullOrEmpty(session))
                    {
                        #if UNITY_EDITOR
                        SaveSession(request.session);
                        #endif
                        
                        response.session = request.session;
                    }
                    else
                    {
                        response.session = session;
                    }

                    response.result = "unity";
                    break;
                case "exist":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        gameObject => true.ToString());
                    break;
                case "active":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        gameObject => gameObject.activeInHierarchy.ToString());
                    break;
                case "onscreen":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => ScreenHelper.IsOnScreen(gameObject).ToString());
                    break;
                case "graphicclickable":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => ScreenHelper.IsGraphicClickable(gameObject).ToString());
                    break;
                case "physicclickable":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => ScreenHelper.IsPhysicClickable(gameObject).ToString());
                    break;
                case "getcomponent":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var component = gameObject.GetComponent(request.value);
                        return component != null ? JsonUtility.ToJson(component) : "null";
                    });
                    break;

                case "click":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var pointer = new PointerEventData(EventSystem.current);
                        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
                        return Constants.ErrorMessages.SuccessResult;
                    });
                    break;
                case "isrendering":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        go =>
                        {
                            var renderer = go.GetComponent<Renderer>();
                            if (renderer != null)
                                return renderer.isVisible.ToString();

                            return false.ToString();

                        });
                    break;
                case "count":
                    response.result = MainThreadHelper.ExecuteGameObjectsEmulation(request.root, request.name, request.parent, request.upath, goList => goList.Count.ToString());
                    break;
                case "deleteplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        PlayerPrefs.DeleteKey(request.key);
                        PlayerPrefs.Save();
                    });
                    break;
                case "deleteallprefs":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                    });
                    break;
                case "getfloatplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => PlayerPrefs.GetFloat(request.key).ToString(CultureInfo.InvariantCulture));
                    break;
                case "getintplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => PlayerPrefs.GetInt(request.key).ToString());
                    break;
                case "getstringplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => PlayerPrefs.GetString(request.key));
                    break;
                case "setfloatplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        var result = Constants.ErrorMessages.SuccessResult;
                        try
                        {
                            PlayerPrefs.SetFloat(request.key, float.Parse(request.value));
                            PlayerPrefs.Save();
                        }
                        catch (Exception e)
                        {
                            result = e.ToString();
                        }
                        return result;
                    });
                    break;
                case "setintplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        var result = Constants.ErrorMessages.SuccessResult;
                        try
                        {
                            PlayerPrefs.SetInt(request.key, int.Parse(request.value));
                            PlayerPrefs.Save();
                        }
                        catch (Exception e)
                        {
                            result = e.ToString();
                        }
                        return result;
                    });
                    break;
                case "setstringplayerpref":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        var result = Constants.ErrorMessages.SuccessResult;
                        try
                        {
                            PlayerPrefs.SetFloat(request.key, float.Parse(request.value));
                            PlayerPrefs.Save();
                        }
                        catch (Exception e)
                        {
                            result = e.ToString();
                        }
                        return result;
                    });
                    break;
                case "playerprefhaskey":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        string result;
                        try
                        {
                            result = PlayerPrefs.HasKey(request.key).ToString();
                        }
                        catch (Exception e)
                        {
                            result = e.ToString();
                        }
                        return result;
                    });
                    break;
                case "getcoordinates":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var position = ScreenHelper.GetPositionOnScreen(gameObject);
                        var coordinates = new ScreenCoordinates {X = position.x, Y = position.y};
                        return JsonUtility.ToJson(coordinates);
                    });
                    break;

                case "swipe":
                    var swipeDirection = Vector2.zero;
                    switch (request.value)
                    {
                        case "up":
                            swipeDirection = Vector2.up;
                            break;
                        case "down":
                            swipeDirection = Vector2.down;
                            break;
                        case "left":
                            swipeDirection = Vector2.left;
                            break;
                        case "right":
                            swipeDirection = Vector2.right;
                            break;
                    }

                    swipeDirection *= 100;

                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => {
                        var pointer = new PointerEventData(EventSystem.current);
                        gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DragCoroutine(gameObject, pointer, (Vector2)ScreenHelper.GetPositionOnScreen(gameObject) + swipeDirection * 2));

                        return Constants.ErrorMessages.SuccessResult;
                    });
                    break;
                
                case "dragto":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => {
                        var screenCoordinates = new ScreenCoordinates();
                        JsonUtility.FromJsonOverwrite(request.value, screenCoordinates);
                        var pointer = new PointerEventData(EventSystem.current);
                        gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DragCoroutine(gameObject, pointer,
                            new Vector2 {x = screenCoordinates.X, y = screenCoordinates.Y}));

                        return "OK";
                    });
                    break;


                case "sendkeys":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var input = gameObject.GetComponent<InputField>();
                        if (input != null)
                        {
                            input.text = request.value;
                        }
                        else
                        {
                            return "input not found";
                        }

                        return Constants.ErrorMessages.SuccessResult;
                    });
                    break;

                case "startplaymode":
#if UNITY_EDITOR
                    EditorApplication.update += StartPlayMode;
                    response.result = Constants.ErrorMessages.SuccessResult;
#else
                    response.result = Constants.ErrorMessages.MethodIsNotSupported;
#endif    
                    break;
                case "stopplaymode":
#if UNITY_EDITOR                    
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() =>
                    {
                        //EditorApplication.UnlockReloadAssemblies();
                        EditorApplication.isPlaying = false;
                    });
#else                  
                    response.result = Constants.ErrorMessages.MethodIsNotSupported;
#endif    
                    break;
                case "ping":
                    response.result = "pong";
                    break;
                case "takescreenshot":
                    var path = request.value;
                    MainThreadQueue.QueueOnMainThread(() => { TakeScreenshot(path); });
                    response.result = Constants.ErrorMessages.SuccessResult;
                    break;
                case "isplaymode":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => Application.isPlaying.ToString());
                    break;

                default:
                    response.result = "Unknown method " + request.method + ".";
                    break;
            }

            return response;
        }

        private static IEnumerator DragCoroutine(GameObject go, PointerEventData dragPointer, Vector2 screenCoordinates) 
        {
            var currentCoordinates = ScreenHelper.GetPositionOnScreen(go);
            dragPointer.position = currentCoordinates;
            var dragDelta = ((Vector2)currentCoordinates - screenCoordinates) / 2;
            dragPointer.delta = dragDelta;

            ExecuteEvents.Execute(go, dragPointer, ExecuteEvents.beginDragHandler);

            for (var i = 0; i < 2; i++) {
                ExecuteEvents.Execute(go, dragPointer, ExecuteEvents.dragHandler);

                dragPointer.position += dragDelta;
                yield return null;
            }

            ExecuteEvents.Execute(go, dragPointer, ExecuteEvents.endDragHandler);
        }

        private static void StartPlayMode()
        {
#if UNITY_EDITOR            
            EditorApplication.update -= StartPlayMode;
			
			ProgressReset.ClearProgress();
            
            //EditorApplication.LockReloadAssemblies();
            EditorApplication.isPlaying = true;
#endif            
        }

        private static void TakeScreenshot(string pathName) 
        {
            var cam = Camera.main.GetComponent<Camera>();
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cam.targetTexture = renderTexture;
            cam.Render();
            cam.targetTexture = null;

            RenderTexture.active = renderTexture;
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();
            RenderTexture.active = null;

            //Encode screenshot to PNG
            byte[] bytes = screenshot.EncodeToPNG();
            UnityEngine.Object.Destroy(screenshot);
            File.WriteAllBytes(pathName, bytes);
        }

        private static void SaveSession(string session)
        {
            var bf = new BinaryFormatter();
            var file = File.Create(Directory.GetCurrentDirectory() + "/session.data");

            var sessionData = new SessionInfo { Session = session };
            bf.Serialize(file, sessionData);
            file.Close();
        }

        private static string GetSession()
        {
            string session = null;
#if UNITY_EDITOR
            if (File.Exists(Directory.GetCurrentDirectory() + "/session.data"))
            {
                var bf = new BinaryFormatter();
                var file = File.Open(Directory.GetCurrentDirectory()+ "/session.data", FileMode.Open);

                session = ((SessionInfo)bf.Deserialize(file)).Session;
                file.Close();
            }
#endif

            return session;
        }

        private static void DeleteSession()
        {
            File.Delete(Directory.GetCurrentDirectory() + "/session.data");
        }
    }
}
