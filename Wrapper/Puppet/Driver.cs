using System.Threading;
using System;

namespace Puppet
{
    internal static class Driver
    {
        private static ThreadLocal<DriverHandler> _handler = new ThreadLocal<DriverHandler>();

        internal static DriverHandler Instance
        {
            get
            {
                if (_handler.Value == null)
                {
                    if (Configuration.DriverType == DriverTypes.UnrealEngine)
                        _handler.Value = new DriverHandler();
                    else
                        throw new NotSupportedException($"{Configuration.DriverType} not supported by Puppet framework right now");
                }

                return _handler.Value;
            }
        }
    }
}
