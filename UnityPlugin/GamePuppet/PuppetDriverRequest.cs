using System;

namespace Assets.Plugins.GamePuppet
{
    [Serializable]
    public class PuppetDriverRequest
    {
        public string method;
        public string name;
        public string parent;
        public string path;
        public string value;

        public override string ToString()
        {
            return string.Format("method: {0}, name: {1}, parent: {2}, path: {3}, value: {4}", method, name, parent, path, value);
        }
    }
}
