using System;

namespace Puppetry.Puppeteer
{
    public static class Configuration
    {
        static Configuration()
        {
            BaseUrl = "http://127.0.0.1";
            Port = "7111";
            TimeoutMs = 60000;
            StartPlayModeTimeoutMs = 240000;
            SessionTimeoutMs = 120000;
            //DriverType = DriverTypes.Unity;
            PollingStratagy = PollingStrategies.Constant;
        }

        internal static string BaseUrl { get; private set; }
        internal static string Port { get; private set; }
        internal static int TimeoutMs { get; private set; }
        internal static int SessionTimeoutMs { get; private set; }
        internal static int StartPlayModeTimeoutMs { get; private set; }
        //internal static DriverTypes DriverType { get; private set; }
        internal static PollingStrategies PollingStratagy { get; private set; }

        public static void Set(Settings setting, object value)
        {
            switch (setting)
            {
                case Settings.BaseUrl:
                    BaseUrl = value.ToString();
                    break;
                case Settings.Port:
                    Port = value.ToString();
                    break;
                case Settings.TimeoutMs:
                    TimeoutMs = (int)value;
                    break;
                case Settings.StartPlayModeTimeoutMs:
                    StartPlayModeTimeoutMs = (int)value;
                    break;
                case Settings.SessionTimeoutMs:
                    SessionTimeoutMs = (int)value;
                    break;
                /*case Settings.Driver:
                    DriverType = (DriverTypes)Enum.Parse(typeof(DriverTypes), value.ToString());
                    break;*/
                case Settings.PollingStrategy:
                    PollingStratagy = (PollingStrategies)Enum.Parse(typeof(PollingStrategies), value.ToString());
                    break;
            }
        }
    }

    public enum Settings
    {
        BaseUrl,
        Port,
        TimeoutMs,
        SessionTimeoutMs,
        StartPlayModeTimeoutMs,
        //Driver,
        PollingStrategy
    }

    /*public enum DriverTypes
    {
        UnrealEngine,
        Unity
    }*/
    
    public enum PollingStrategies
    {
        Constant,
        Progressive
    }
}
