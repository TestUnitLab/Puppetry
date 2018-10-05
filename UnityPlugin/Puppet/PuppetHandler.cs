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
    public class PuppetHandler
    {
        internal static DriverResponse HandleDriverRequest(DriverRequest request)
        {
            var response = new DriverResponse {method = request.method };
            var swipeDirection = Vector2.zero;
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
                        gameObject => true.ToString(), false.ToString());
                    break;
                case "active":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath,
                        gameObject => gameObject.activeInHierarchy.ToString(), false.ToString());
                    break;
                case "getcomponent":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, gameObject =>
                    {
                        var component = gameObject.GetComponent(request.value);
                        return component != null ? EditorJsonUtility.ToJson(component) : "null";
                    });
                    break;

                case "click":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, go =>
                    {
                        var pointer = new PointerEventData(EventSystem.current);
                        ExecuteEvents.Execute(go, pointer, ExecuteEvents.pointerClickHandler);
                        return "success";
                    });
                    break;
                case "isrendering":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent,
                        request.upath,
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

                case "swipeleft":
                    swipeDirection = Vector2.left;
                    goto case "swipe";
                case "swiperight":
                    swipeDirection = Vector2.right;
                    goto case "swipe";
                case "swipeup":
                    swipeDirection = Vector2.up;
                    goto case "swipe";
                case "swipedown":
                    swipeDirection = Vector2.down;
                    goto case "swipe";

                case "swipe":
                    swipeDirection *= 100;

                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, go =>
                    {
                        var pointer = new PointerEventData(EventSystem.current);

                        var rt = (RectTransform) go.transform;

                        Vector3[] corners = new Vector3[4];
                        rt.GetWorldCorners(corners);
                        var bottomLeft = Camera.main.WorldToScreenPoint(corners[0]);
                        var topLeft = Camera.main.WorldToScreenPoint(corners[1]);
                        var topRight = Camera.main.WorldToScreenPoint(corners[2]);
                        var bottomRight = Camera.main.WorldToScreenPoint(corners[3]);

                        var center = new Vector2(topLeft.x + (bottomRight.x - topLeft.x) / 2,
                            bottomRight.y + (topLeft.y - bottomRight.y) / 2);

                        go.GetComponent<MonoBehaviour>()
                            .StartCoroutine(DragCoroutine(go, pointer, center, swipeDirection));

                        return "success";
                    });
                    break;


                case "sendkeys":
                    response.result = ExecuteGameObjectEmulation(request.root, request.name, request.parent, request.upath, go =>
                    {
                        var input = go.GetComponent<InputField>();
                        if (input != null)
                        {
                            input.text = request.value;
                        }
                        else
                        {
                            return "input not found";
                        }

                        return "success";
                    });
                    break;

                case "startplaymode":
                    EditorApplication.update += StartPlayMode;
                    response.result = "success";
                    break;
                case "stopplaymode":
                    response.result = InvokeOnMainThreadAndWait(() =>
                    {
                        EditorApplication.UnlockReloadAssemblies();
                        EditorApplication.isPlaying = false;
                    });

                    break;
                case "ping":
                    response.result = "pong";
                    break;
                case "takescreenshot":
                    var path = request.value;
                    DriverProcessor.QueueOnMainThread(() => { TakeScreenshot(path); });
                    response.result = "success";
                    break;

                default:
                    response.result = "Unknown method " + request.method + ".";
                    break;
            }

            return response;
        }

        private static IEnumerator DragCoroutine(GameObject go, PointerEventData dragPointer, Vector2 goCenter, Vector2 dragDelta)
        {
            dragPointer.position = goCenter;
            dragPointer.delta = dragDelta;

            ExecuteEvents.Execute(go, dragPointer, ExecuteEvents.beginDragHandler);

            for (var i = 0; i < 2; i++)
            {
                ExecuteEvents.Execute(go, dragPointer, ExecuteEvents.dragHandler);

                dragPointer.position += dragDelta;
                yield return null;
            }

            ExecuteEvents.Execute(go, dragPointer, ExecuteEvents.endDragHandler);
        }

        private static void StartPlayMode()
        {
            EditorApplication.update -= StartPlayMode;
            
            EditorApplication.LockReloadAssemblies();
            EditorApplication.isPlaying = true;
        }

        private static string ExecuteGameObjectEmulation(string rootName, string nameOrPath, string parent, string upath, Func<GameObject, string> onComplete, string notFoundMsg = null)
        {
            // event used to wait the answer from the main thread.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            string response = "";
            DriverProcessor.QueueOnMainThread(() =>
            {
                try
                {
                    GameObject go;
                    if (!string.IsNullOrEmpty(upath))
                        go = FindGameObjectHelper.FindGameObjectByUPath(upath);
                    else
                        go = FindGameObjectHelper.FindGameObject(rootName, nameOrPath, parent);
                    
                    if (go != null)
                    {
                        response = onComplete(go);
                    }
                    else
                    {
                        if (notFoundMsg != null)
                        {
                            response = notFoundMsg;
                        }
                        else
                        {
                            response = "not found (" + (parent != null ? parent + "/" : "") + nameOrPath + ")";
                        }
                    }
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
            UnityDriver.QueueOnMainThread(() => {
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

            string response = "success";
            DriverProcessor.QueueOnMainThread(() =>
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
            var cam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
