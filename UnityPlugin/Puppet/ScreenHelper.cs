using UnityEngine;

namespace Puppetry.Puppet
{
    public static class ScreenHelper
    {
        public static Vector3 GetPositionOnScreen(GameObject gameObject)
        {
            var camera = Camera.main.GetComponent<Camera>();
            var canvasParent = gameObject.GetComponentInParent<Canvas>();
            if (canvasParent != null)
            {
                if (canvasParent.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    var vector3S = new Vector3[4];
                    gameObject.GetComponent<RectTransform>().GetWorldCorners(vector3S);
                    var center = new Vector3((vector3S[0].x + vector3S[2].x) / 2, (vector3S[0].y + vector3S[2].y) / 2, (vector3S[0].z + vector3S[2].z) / 2);
                    return canvasParent.worldCamera.WorldToScreenPoint(center);
                }
                if (gameObject.GetComponent<RectTransform>() != null)
                {
                    return gameObject.GetComponent<RectTransform>().position;
                }
                return camera.WorldToScreenPoint(gameObject.transform.position);
            }

            if (gameObject.GetComponent<Collider>() != null)
            {
                return camera.WorldToScreenPoint(gameObject.GetComponent<Collider>().bounds.center);
            }

            return camera.WorldToScreenPoint(gameObject.transform.position);
        }
    }
}