using System;

namespace Puppetry.Puppet
{
    [Serializable]
    public class DriverRequest
    {
        public string session;
        public string method;
        public string root;
        public string name;
        public string parent;
        public string upath;
        public string value;

        public override string ToString()
        {
            return string.Format("session: {0}, root: {1}, method: {2}, name: {3}, parent: {4}, upath: {5}, value: {6}", session, root, method, name, parent, upath, value);
        }
    }
}
