using System;

namespace GamePuppet
{
    [Serializable]
    public class PuppetDriverRequest
    {
        public string session;
        public string method;
        public string name;
        public string parent;
        public string value;

        public override string ToString()
        {
            return string.Format("session: {0}, method: {1}, name: {2}, parent: {3}, path: {4}, value: {5}", session, method, name, parent, value);
        }
    }
}
