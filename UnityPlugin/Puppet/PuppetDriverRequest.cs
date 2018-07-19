using System;

namespace Puppetry.Puppet
{
    [Serializable]
    public class PuppetDriverRequest
    {
        public string session;
        public string method;
        public string root;
        public string name;
        public string parent;
        public string value;

        public override string ToString()
        {
            return string.Format("session: {0}, root: {1}, method: {2}, name: {3}, parent: {4}, value: {5}", session, root, method, name, parent, value);
        }
    }
}
