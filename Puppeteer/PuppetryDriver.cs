using Puppetry.Puppeteer.Driver;
using System.Threading;

namespace Puppetry.Puppeteer
{
    public static class PuppetryDriver
    {
        private static ThreadLocal<PuppetDriverClient> _handler = new ThreadLocal<PuppetDriverClient>();

        public static void ReleaseCurrentSession()
        {
            Instance.ReleaseSession();
            Clear();
        }

        public static void ReleaseAllSessions()
        {
            PuppetDriverClient.ReleaseAllSessions();
            Clear();
        }

        internal static PuppetDriverClient Instance
        {
            get
            {
                if (_handler.Value == null)
                {
                    _handler.Value = new PuppetDriverClient();
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
