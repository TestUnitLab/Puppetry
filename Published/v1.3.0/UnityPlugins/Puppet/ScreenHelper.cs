using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Puppetry.Puppet
{
    public static class ScreenHelper
    {
        public static bool IsGraphicClickable(GameObject gameObject)
        {
            return IsRaycasted<GraphicRaycaster>(gameObject);
        }

        public static bool IsPhysicClickable(GameObject gameObject)
        {
            return IsRaycasted<PhysicsRaycaster>(gameObject);
        }

        public static bool IsOnScreen(GameObject gameObject)
        {
            var result = false;

            //Check if any camera can see the object))
            foreach (var camera in Camera.allCameras)
            {
                var position = GetPositionOnScreen(gameObject, camera);

                result = (position.x > 0 && position.y > 0 && position.x < Screen.width && position.y < Screen.height && position.z >= 0);

                if (result)
                    break;
            }

            return result;
        }

        public static Vector3 GetPositionOnScreen(GameObject gameObject, Camera camera)
        {
            var canvasParent = gameObject.GetComponentInParent<Canvas>();
            if (canvasParent != null)
            {
                var rectTransform = gameObject.GetComponent<RectTransform>();

                if (canvasParent.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    if (rectTransform != null)
                    {
                        var vector3S = new Vector3[4];
                        rectTransform.GetWorldCorners(vector3S);
                        var center = new Vector3((vector3S[0].x + vector3S[2].x) / 2,
                            (vector3S[0].y + vector3S[2].y) / 2, (vector3S[0].z + vector3S[2].z) / 2);
                        return canvasParent.worldCamera.WorldToScreenPoint(center);
                    }
                }

                if (rectTransform != null)
                {
                    return rectTransform.position;
                }

                return camera.WorldToScreenPoint(gameObject.transform.position);
            }

            var collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                return camera.WorldToScreenPoint(collider.bounds.center);
            }

            return camera.WorldToScreenPoint(gameObject.transform.position);
        }

        private static bool IsRaycasted<T>(GameObject gameObject) where T : BaseRaycaster
        {
            var result = false;
            var position = GetPositionOnScreen(gameObject, Camera.main);

            if (IsOnScreen(gameObject))
            {
                var raycaster = gameObject.GetComponentInParent<T>();
                var ped = new PointerEventData(null);
                ped.position = position;
                var hits = new List<RaycastResult>();
                raycaster.Raycast(ped, hits);

                if (hits.Count > 0 && hits[0].gameObject.name == gameObject.name)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}