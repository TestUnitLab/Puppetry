using System;

namespace Puppet
{
    public class Configuration
    {
        internal static string BaseUrl;
        internal static string Port;
        internal static int TimeoutMs;
        internal static DriverTypes DriverType;

        public void Set(Settings setting, object value)
        {
            switch (setting)
            {
                case Settings.BaseUrl:
                    BaseUrl = (string)value;
                    break;
                case Settings.Port:
                    Port = (string)value;
                    break;
                case Settings.TimeoutMs:
                    TimeoutMs = (int)value;
                    break;
                case Settings.Driver:
                    DriverType = (DriverTypes)Enum.Parse(typeof(DriverTypes), value.ToString());
                    break;
            }
        }
    }

    public enum Settings
    {
        BaseUrl,
        Port,
        TimeoutMs,
        Driver
    }

    public enum DriverTypes
    {
        UnrealEngine,
        Unity
    }
}
