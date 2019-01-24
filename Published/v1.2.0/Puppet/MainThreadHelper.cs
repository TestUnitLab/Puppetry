using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Puppetry.Puppet.Contracts;

namespace Puppetry.Puppet
{
    public static class MainThreadHelper
    {
        public static string ExecuteGameObjectEmulation(string upath, Func<GameObject, string> onComplete)
        {
            // event used to wait the answer from the main thread.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            var response = ErrorMessages.MainThreadIsUnavailable; // If response was not changed then MainThreadHelper is not initialized.
            MainThreadQueue.QueueOnMainThread(() =>
            {
                try
                {
                    var gameObject = FindGameObjectHelper.FindGameObjectByUPath(upath);

                    if (gameObject != null)
                        response = onComplete(gameObject);
                    else
                        response = ErrorMessages.GameObjectWasNotFound;
                }
                catch (Exception e)
                {
                    Utils.Logger.Log(e);
                    response = e.Message;
                }
                finally
                {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread or 5 seconds timeout
            autoEvent.WaitOne(5000);

            return response;
        }

        public static string ExecuteGameObjectsEmulation(string upath, Func<List<GameObject>, string> onComplete)
        {
            var autoEvent = new AutoResetEvent(false);

            var response = ErrorMessages.MainThreadIsUnavailable; // If response was not changed then MainThreadHelper is not initialized.
            MainThreadQueue.QueueOnMainThread(() =>
            {
                try
                {
                    var listOfGOs = FindGameObjectHelper.FindGameObjectsByUPath(upath);

                    response = onComplete(listOfGOs);

                }
                catch (Exception e)
                {
                    Utils.Logger.Log(e);
                    response = e.Message;
                }
                finally
                {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread or 5 seconds timeout
            autoEvent.WaitOne(5000);

            return response;
        }

        public static string InvokeOnMainThreadAndWait(Action action)
        {
            // event used to wait the answer from the main thread.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            var response = ErrorMessages.MainThreadIsUnavailable; // If response was not changed then MainThreadHelper is not initialized.
            MainThreadQueue.QueueOnMainThread(() =>
            {
                try
                {
                    action();
                    response = ErrorMessages.SuccessResult;
                }
                catch (Exception e)
                {
                    Utils.Logger.Log(e);
                    response = e.Message;
                }
                finally
                {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread or 5 seconds timeout
            autoEvent.WaitOne(5000);

            return response;
        }

        public static string InvokeOnMainThreadAndWait(Func<string> action)
        {
            // event used to wait the answer from the main thread.
            AutoResetEvent autoEvent = new AutoResetEvent(false);

            var response = ErrorMessages.MainThreadIsUnavailable; // If response was not changed then MainThreadHelper is not initialized.
            MainThreadQueue.QueueOnMainThread(() =>
            {
                try
                {
                    response = action.Invoke();
                }
                catch (Exception e)
                {
                    Utils.Logger.Log(e);
                    response = e.Message;
                }
                finally
                {
                    // set the event to "unlock" the thread
                    autoEvent.Set();
                }
            });

            // wait for the end of the 'action' executed in the main thread or 5 seconds timeout
            autoEvent.WaitOne(5000);

            return response;
        }
    }
}
