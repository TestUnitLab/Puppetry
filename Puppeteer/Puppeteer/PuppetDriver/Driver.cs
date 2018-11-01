using System.Threading;

namespace Puppetry.Puppeteer.PuppetDriver
{
    public static class Driver
    {
        private static ThreadLocal<DriverHandler> _handler = new ThreadLocal<DriverHandler>();

        public static void KillSession()
        {
            Instance.KillSession();
            Clear();
        }
        
        public static void KillAllSessions()
        {
            Instance.KillAllSessions();
            Clear();
        }
        
        internal static DriverHandler Instance
        {
            get
            {
                if (!_handler.IsValueCreated && _handler.Value == null)
                {
                    _handler.Value = new DriverHandler();
                }

                return _handler.Value;
            }
        }

        internal static void Clear()
        {
            _handler.Value = null;
        }
    }
}
