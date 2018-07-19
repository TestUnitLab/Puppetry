using Puppetry.Puppeteer.Conditions;
using Puppetry.Puppeteer.PuppetDriver;
using Puppetry.Puppeteer.Utils;

namespace Puppetry.Puppeteer.Puppet
{
    public class GameObject
    {
        internal string Name;
        internal string Root;
        internal string Parent;

        public GameObject(string root, string name)
        {
            Root = root;
            Name = name;
        }

        public GameObject(string root, string name, string parent)
        {
            Root = root;
            Name = name;
            Parent = parent;
        }

        public void Click()
        {
            Driver.Instance.Click(Root, Name, Parent);
        }

        public void SendKeys(string value)
        {
            Driver.Instance.SendKeys(value, Root, Name, Parent);
        }

        public bool Exists()
        {
            return Driver.Instance.Exist(Root, Name, Parent);
        }

        public bool IsActive()
        {
            return Driver.Instance.Active(Root, Name, Parent);
        }

        public void Should(Condition condition)
        {
            Wait.For(() => condition.Invoke(this),
                () =>
                    $"Timed out after {Configuration.TimeoutMs / 1000} seconds while waiting for Condition: {condition}",
                Configuration.TimeoutMs);
        }

        public void ShouldNot(Condition condition)
        {
            Wait.For(() => !condition.Invoke(this),
                () =>
                    $"Timed out after {Configuration.TimeoutMs / 1000} seconds while waiting for Condition Not fulfilled: {condition}",
                Configuration.TimeoutMs);
        }
    }
}
