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
        internal string UPath;

        public GameObject() { }

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

        public GameObject FindByUPath(string upath)
        {
            UPath = upath;

            return this;
        }
        
        public GameObject FindByName(string root, string name)
        {
            Root = root;
            Name = name;

            return this;
        }
        
        public GameObject FindByNameAndParent(string root, string name, string parent)
        {
            Root = root;
            Name = name;
            Parent = parent;

            return this;
        }
        
        public bool Exists => Driver.Instance.Exist(Root, Name, Parent, UPath);
        
        public bool IsActiveInHierarchy => Driver.Instance.Active(Root, Name, Parent, UPath);
        
        public bool IsRendering => Driver.Instance.IsRendering(Root, Name, Parent, UPath);
        
        public int Count => Driver.Instance.Count(Root, Name, Parent, UPath);

        public void Click()
        {
            Driver.Instance.Click(Root, Name, Parent, UPath);
        }

        public void SendKeys(string value)
        {
            Driver.Instance.SendKeys(value, Root, Name, Parent, UPath);
        }
        
        public string GetComponent(string component)
        {
            return Driver.Instance.GetComponent(Root, Name, Parent, UPath, component);
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
