using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puppetry.Puppet
{
    public static class DriverHandler
    {
        const string SuccessResult = "success";

        internal static DriverResponse HandleDriverRequest(DriverRequest request)
        {
            var response = new DriverResponse {method = request.method };
            var session = GetSession();

            switch (request.method.ToLowerInvariant())
            {
                case "registereditor":
                    if (string.IsNullOrEmpty(session))
                    {
                        SaveSession(request.session);
                        response.session = request.session;
                    }
                    else
                    {
                        response.session = session;
                    }

                    response.result = "unity";
                    break;
                case "exist":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        gameObject => true.ToString());
                    break;
                case "active":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        gameObject => gameObject.activeInHierarchy.ToString());
                    break;
                case "onscreen":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => ScreenHelper.IsOnScreen(gameObject).ToString());
                    break;
                case "graphicclickable":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => ScreenHelper.IsGraphicClickable(gameObject).ToString());
                    break;
                case "getcomponent":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var component = gameObject.GetComponent(request.value);
                        return component != null ? EditorJsonUtility.ToJson(component) : "null";
                    });
                    break;

                case "click":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var pointer = new PointerEventData(EventSystem.current);
                        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
                        return SuccessResult;
                    });
                    break;
                case "isrendering":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        go =>
                        {
                            var renderer = go.GetComponent<Renderer>();
                            if (renderer != null)
                                return renderer.isVisible.ToString();

                            return false.ToString();

                        });
                    break;
                case "count":
                    response.result = ExecuteGameObjectsEmulation(request.root, request.name, request.parent, request.upath, goList => goList.Count.ToString());
                    break;
                case "deletepref":
                    response.result = InvokeOnMainThreadAndWait(() =>
                    {
                        PlayerPrefs.DeleteKey(request.value);
                        PlayerPrefs.Save();
                    });
                    break;
                case "deleteallprefs":
                    response.result = InvokeOnMainThreadAndWait(() =>
                    {
                        PlayerPrefs.DeleteAll();
                        PlayerPrefs.Save();
                    });
                    break;
                case "getcoordinates":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
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

                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => {
                        var pointer = new PointerEventData(EventSystem.current);
                        gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DragCoroutine(gameObject, pointer, (Vector2)ScreenHelper.GetPositionOnScreen(gameObject) + swipeDirection * 2));

                        return SuccessResult;
                    });
                    break;
                
                case "dragto":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject => {
                        var screenCoordinates = new ScreenCoordinates();
                        EditorJsonUtility.FromJsonOverwrite(request.value, screenCoordinates);
                        var pointer = new PointerEventData(EventSystem.current);
                        gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DragCoroutine(gameObject, pointer,
                            new Vector2 {x = screenCoordinates.X, y = screenCoordinates.Y}));

                        return "OK";
                    });
                    break;


                case "sendkeys":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
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

                        return SuccessResult;
                    });
                    break;

                case "startplaymode":
                    EditorApplication.update += StartPlayMode;
                    response.result = SuccessResult;
                    break;
                case "stopplaymode":
                    response.result = InvokeOnMainThreadAndWait(() =>
                    {
                        //EditorApplication.UnlockReloadAssemblies();
                        EditorApplication.isPlaying = false;
                    });

                    break;
                case "ping":
                    response.result = "pong";
                    break;
                case "takescreenshot":
                    var path = request.value;
                    MainThreadHelper.QueueOnMainThread(() => { TakeScreenshot(path); });
                    response.result = SuccessResult;
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
            EditorApplication.update -= StartPlayMode;
            
            //EditorApplication.LockReloadAssemblies();
            EditorApplication.isPlaying = true;
        }

        private static string ExecuteGameObjectEmulation(string rootName, string nameOrPath, string parent, string upath, Func<GameObject, string> onComplete)
        {
            // event used to wait the answer from the main thread.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            string response = "";
            MainThreadHelper.QueueOnMainThread(() =>
            {
                try
                {
                    GameObject go;
                    if (!string.IsNullOrEmpty(upath))
                        go = FindGameObjectHelper.FindGameObjectByUPath(upath);
                    else
                        go = FindGameObjectHelper.FindGameObject(rootName, nameOrPath, parent);
                    
                    if (go != null)
                        response = onComplete(go);
                    else
                        response = "GameObject was not found";
                }
                catch (Exception e)
                {
                    Log(e);
                    response = e.Message;
                }
                finally
                {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread
            autoEvent.WaitOne();

            return response;
        }
        
        private static string ExecuteGameObjectsEmulation(string nameOrPath, string parent, string root, string upath, Func<List<GameObject>, string> onComplete)
        {
            var autoEvent = new AutoResetEvent(false);

            var response = "";
            MainThreadHelper.QueueOnMainThread(() => {
                try
                {
                    List<GameObject> listOfGOs;
                    if (!string.IsNullOrEmpty(upath))
                    {
                        listOfGOs = FindGameObjectHelper.FindGameObjectsByUPath(upath);
                    }
                    else
                    {
                        listOfGOs = FindGameObjectHelper.GetGameObjects(nameOrPath, root, parent);
                    }
                    
                    response = onComplete(listOfGOs);

                } catch (Exception e) {
                    Log(e);
                    response = e.Message;
                } finally {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread
            autoEvent.WaitOne();

            return response;
        }

        private static string InvokeOnMainThreadAndWait(Action action)
        {
            // event used to wait the answer from the main thread.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            string response = SuccessResult;
            MainThreadHelper.QueueOnMainThread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Log(e);
                    response = e.Message;
                }
                finally
                {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread
            autoEvent.WaitOne();

            return response;
        }

        public static void Log(string msg)
        {
            Debug.Log(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " [Puppet] " + msg);
        }

        private static void Log(Exception e)
        {
            Debug.Log(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " [Puppet] " + e);
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
            if (File.Exists(Directory.GetCurrentDirectory() + "/session.data"))
            {
                var bf = new BinaryFormatter();
                var file = File.Open(Directory.GetCurrentDirectory()+ "/session.data", FileMode.Open);

                session = ((SessionInfo)bf.Deserialize(file)).Session;
                file.Close();
            }

            return session;
        }

        private static void DeleteSession()
        {
            File.Delete(Directory.GetCurrentDirectory() + "/session.data");
        }
    }
}
