using System;

namespace Puppetry.Puppet.Contracts
{
    [Serializable]
    public class DriverRequest
    {
        public string session;
        public string method;
        public string upath;
        public string key;
        public string value;

        public override string ToString()
        {
            return string.Format("session: {0}, method: {1}, upath: {2}, key: {3}, value: {4}", session, method, upath, key, value);
        }
    }
}
