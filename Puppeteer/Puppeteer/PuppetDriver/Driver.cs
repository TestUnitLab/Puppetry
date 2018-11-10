using System.Threading;

namespace Puppetry.Puppeteer.PuppetDriver
{
    public static class Driver
    {
        private static ThreadLocal<DriverHandler> _handler = new ThreadLocal<DriverHandler>();

        public static void ReleaseSession()
        {
            Instance.ReleaseSession();
            Clear();
        }
        
        public static void ReleaseAllSessions()
        {
            DriverHandler.ReleaseAllSessions();
            Clear();
        }
        
        internal static DriverHandler Instance
        {
            get
            {
                if (_handler.Value == null)
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
