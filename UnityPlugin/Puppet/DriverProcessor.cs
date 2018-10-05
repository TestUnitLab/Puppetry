using UnityEngine;
using System.Collections.Generic;
using System;

namespace Puppetry.Puppet
{
    public class DriverProcessor : MonoBehaviour
    {
        private readonly List<Action> requestedActions = new List<Action>();
        private readonly List<Action> currentActions = new List<Action>();

        private static DriverProcessor Instance { get; set; }

        private DriverProcessor()
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