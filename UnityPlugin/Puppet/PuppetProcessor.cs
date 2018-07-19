using UnityEngine;
using System.Collections.Generic;
using System;

namespace Puppetry.Puppet
{
    public class PuppetProcessor : MonoBehaviour
    {
        private readonly List<Action> requestedActions = new List<Action>();
        private readonly List<Action> currentActions = new List<Action>();

        private static PuppetProcessor Instance { get; set; }

        private PuppetProcessor()
        {
            Instance = this;
        }

        public static void QueueOnMainThread(Action action)
        {
            lock (Instance.requestedActions)
            {
                Instance.requestedActions.Add(action);
            }
        }

        private static GameObject FindGameObjectIncludingInactive(string rootName, string name)
        {
            return FindGameObjectIncludingInactive(GameObject.Find(rootName), name);
        }

        private static GameObject FindGameObjectIncludingInactive(GameObject root, string name)
        {
            var found = FindRecursive(root.transform, name);
            return found != null ? found.gameObject : null;
        }

        private static Transform FindRecursive(Transform root, string name)
        {
            var result = root.Find(name);
            if (result != null)
            {
                return result;
            }

            foreach (Transform child in root)
            {
                result = FindRecursive(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public static GameObject FindGameObject(string rootName, string nameOrPath, string parentName)
        {
            if (string.IsNullOrEmpty(parentName))
            {
                return FindGameObjectIncludingInactive(rootName, nameOrPath);
            }

            var parentgameObject = FindGameObjectIncludingInactive(rootName, parentName);
            if (parentgameObject != null)
            {
                if (nameOrPath.Contains("/"))
                {
                    var transform = parentgameObject.transform.Find(nameOrPath);
                    return transform != null ? transform.gameObject : null;
                }
                else
                {
                    return FindGameObjectIncludingInactive(parentgameObject, nameOrPath);
                }
            }
            else
            {
                return null;
            }
        }

        void Update()
        {
            lock (requestedActions)
            {
                currentActions.Clear();
                currentActions.AddRange(requestedActions);
                requestedActions.Clear();
            }

            foreach (var action in currentActions)
            {
                action.Invoke();
            }
        }
    }
}