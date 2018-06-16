using System.Threading;
using System;

using Puppet.Api;

namespace Puppet
{
    internal static class Driver
    {
        private static ThreadLocal<IApiHandler> _handler = new ThreadLocal<IApiHandler>();

        internal static IApiHandler Instance
        {
            get
            {
                if (_handler.Value == null)
                {
                    if (Configuration.DriverType == DriverTypes.UnrealEngine)
                        _handler.Value = new UnrealApiHandler(Configuration.BaseUrl, Configuration.Port);
                    else
                        throw new NotSupportedException($"{Configuration.DriverType} not supported by Puppet framework right now");
                }

                return _handler.Value;
            }
        }
    }
}
