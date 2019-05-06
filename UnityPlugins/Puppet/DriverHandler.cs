using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Puppetry.Puppet.Contracts;
using TMPro;
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
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath,
                        gameObject => true.ToString());
                    break;
                case "active":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath,
                        gameObject => gameObject.activeInHierarchy.ToString());
                    break;
                case "onscreen":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject => ScreenHelper.IsOnScreen(gameObject).ToString());
                    break;
                case "raycasted":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject =>
                    {
                        string result = null;
                        switch (request.value)
                        {
                            case "graphicraycaster":
                                result = ScreenHelper.IsRaycasted<GraphicRaycaster>(gameObject).ToString();
                                break;
                            case "physicsraycaster":
                                result = ScreenHelper.IsRaycasted<PhysicsRaycaster>(gameObject).ToString();
                                break;
                            case "physics2draycaster":
                                result = ScreenHelper.IsRaycasted<Physics2DRaycaster>(gameObject).ToString();
                                break;
                            default:
                                result = "Error: " + request.value +" is invalid raycaster";
                                break;
                        }

                        return result;
                    });
                    break;
                case "getcomponent":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject =>
                    {
                        var component = gameObject.GetComponent(request.value);
#if UNITY_EDITOR
                        return component != null ? EditorJsonUtility.ToJson(component) : "null";
#else
                        return component != null ? JsonUtility.ToJson(component) : "null";
#endif
                    });
                    break;

                case "click":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject =>
                    {
                        var pointer = new PointerEventData(EventSystem.current);
                        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
                        return ErrorMessages.SuccessResult;
                    });
                    break;
                case "isrendering":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath,
                        go =>
                        {
                            var renderer = go.GetComponent<Renderer>();
                            if (renderer != null)
                                return renderer.isVisible.ToString();

                            return false.ToString();


                        });
                    break;
                case "getscene":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => SceneManager.GetActiveScene().name);
                    break;
                case "openscene":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(
                        () =>
                        {
                            try
                            {
                                SceneManager.LoadScene(request.key);
                                return ErrorMessages.SuccessResult;
                            }
                            catch (Exception e)
                            {
                                return e.ToString();
                            }

                        });
                    break;
                case "count":
                    response.result = MainThreadHelper.ExecuteGameObjectsEmulation(request.upath, goList => goList.Count.ToString());
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
                        var result = ErrorMessages.SuccessResult;
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
                        var result = ErrorMessages.SuccessResult;
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
                        var result = ErrorMessages.SuccessResult;
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
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject =>
                    {
                        var position = ScreenHelper.GetPositionOnScreen(gameObject, Camera.main);
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

                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject => {
                        var pointer = new PointerEventData(EventSystem.current);
                        gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DragCoroutine(gameObject, pointer, (Vector2)ScreenHelper.GetPositionOnScreen(gameObject, Camera.main) + swipeDirection * 2));

                        return ErrorMessages.SuccessResult;
                    });
                    break;
                
                case "dragto":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject => {
                        var screenCoordinates = new ScreenCoordinates();
                        JsonUtility.FromJsonOverwrite(request.value, screenCoordinates);
                        var pointer = new PointerEventData(EventSystem.current);
                        gameObject.GetComponent<MonoBehaviour>().StartCoroutine(DragCoroutine(gameObject, pointer,
                            new Vector2 {x = screenCoordinates.X, y = screenCoordinates.Y}));

                        return "OK";
                    });
                    break;


                case "sendkeys":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject =>
                    {
                        var input = gameObject.GetComponent<InputField>();
                        if (input != null)
                        {
                            input.text = request.value;
                        }
                        else
                        {
                            var tmpInput = gameObject.GetComponent<TMP_InputField>();
                            if (tmpInput != null)
                                tmpInput.text = request.value;
                            else
                                return "input not found";
                        }

                        return ErrorMessages.SuccessResult;
                    });
                    break;

                case "startplaymode":
#if UNITY_EDITOR
                    EditorApplication.update += StartPlayMode;
                    response.result = ErrorMessages.SuccessResult;
#else
                    response.result = ErrorMessages.MethodIsNotSupported;
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
                    response.result = ErrorMessages.MethodIsNotSupported;
#endif
                    break;
                case "ping":
                    response.result = "pong";
                    break;
                case "takescreenshot":
                    var path = request.value;
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => { TakeScreenshot(path); });
                    break;
                case "isplaymode":
                    response.result = MainThreadHelper.InvokeOnMainThreadAndWait(() => Application.isPlaying.ToString());
                    break;
				case "gamecustom":
                    response.result = CustomDriverHandler.ProcessGameCustomMethod(request.key, request.value);
                    break;
                case "gameobjectcustom":
                    response.result = CustomDriverHandler.ProcessGameObjectCustomMethod(request.upath, request.key, request.value);
                    break;
                case "getspritename":
                    response.result = MainThreadHelper.ExecuteGameObjectEmulation(request.upath, gameObject =>
                    {
                        var component = gameObject.GetComponent<SpriteRenderer>();
                        if (component != null)
                            return component.sprite.name;
                        else
                            return "Component was not found";
                    });
                    break;

                default:
                    response.result = "Unknown method " + request.method + ".";
                    break;
            }

            return response;
        }

        private static IEnumerator DragCoroutine(GameObject go, PointerEventData dragPointer, Vector2 screenCoordinates) 
        {
            var currentCoordinates = ScreenHelper.GetPositionOnScreen(go, Camera.main);
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
            
            EditorApplication.isPlaying = true;
#endif
        }

        private static void TakeScreenshot(string pathName)
        {
            foreach (var camera in Camera.allCameras)
            {
                string updatedPathName = null;
                if (Camera.allCamerasCount > 1)
                {
                    updatedPathName = pathName + "ViewedBy" + camera.name;
                    updatedPathName = updatedPathName.Replace(" ", string.Empty);
                }
                else
                {
                    updatedPathName = pathName;
                }
                var screen = GetScreenCapture(camera);

                SaveScreenCapture(screen, updatedPathName);
            }
        }

        private static Texture2D GetScreenCapture(Camera camera)
        {
            var cam = camera.GetComponent<Camera>();
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            cam.targetTexture = renderTexture;
            cam.Render();
            cam.targetTexture = null;

            RenderTexture.active = renderTexture;
            Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenshot.Apply();
            RenderTexture.active = null;

            return screenshot;
        }

        private static void SaveScreenCapture(Texture2D screen, string pathName)
        {
            //Encode screenshot to PNG
            byte[] bytes = screen.EncodeToPNG();
            UnityEngine.Object.Destroy(screen);
            File.WriteAllBytes(pathName + ".png", bytes);
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

        private static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
        {
            if (aBottom.width != aTop.width || aBottom.height != aTop.height)
                throw new InvalidOperationException("AlphaBlend only works with two equal sized images");
            var bData = aBottom.GetPixels();
            var tData = aTop.GetPixels();
            int count = bData.Length;
            var rData = new Color[count];
            for (int i = 0; i < count; i++)
            {
                Color B = bData[i];
                Color T = tData[i];
                float srcF = T.a;
                float destF = 1f - T.a;
                float alpha = srcF + destF * B.a;
                Color R = (T * srcF + B * B.a * destF) / alpha;
                R.a = alpha;
                rData[i] = R;
            }
            var result = new Texture2D(aTop.width, aTop.height);
            result.SetPixels(rData);
            result.Apply();
            return result;
        }
    }
}
