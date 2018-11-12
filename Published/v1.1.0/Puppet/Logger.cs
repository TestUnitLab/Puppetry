using System;
using UnityEngine;

namespace Puppetry.Puppet
{
    public class Logger
    {
        public static void Log(string msg)
        {
            Debug.Log(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " [Puppetry.Puppet] " + msg);
        }

        public static void Log(Exception e)
        {
            Debug.Log(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " [Puppetry.Puppet] " + e);
        }
    }
}
