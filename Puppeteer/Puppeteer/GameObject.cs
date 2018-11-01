using Newtonsoft.Json;
using Puppetry.Puppeteer.Conditions;
using Puppetry.Puppeteer.PuppetDriver;
using Puppetry.Puppeteer.Utils;

namespace Puppetry.Puppeteer
{
    public class GameObject
    {
        private string Name;
        private string Root;
        private string Parent;
        private string UPath;
        internal string LocatorMessage { get; private set;}

        public GameObject() { }

        public GameObject(string root, string name)
        {
            Root = root;
            Name = name;
            LocatorMessage = $"root: {Root} and name: {Name}";
        }

        public GameObject(string root, string name, string parent)
        {
            Root = root;
            Name = name;
            Parent = parent;
            LocatorMessage = $"root: {Root}, name: {Name} and parent: {Parent}";
        }

        public GameObject FindByUPath(string upath)
        {
            UPath = upath;
            LocatorMessage = $"upath: {UPath}";

            return this;
        }
        
        public GameObject FindByName(string root, string name)
        {
            Root = root;
            Name = name;
            LocatorMessage = $"root: {Root} and name: {Name}";

            return this;
        }
        
        public GameObject FindByNameAndParent(string root, string name, string parent)
        {
            Root = root;
            Name = name;
            Parent = parent;
            LocatorMessage = $"root: {Root}, name: {Name} and parent: {Parent}";

            return this;
        }
        
        public bool Exists => Driver.Instance.Exist(Root, Name, Parent, UPath);
        
        public bool IsActiveInHierarchy => Driver.Instance.Active(Root, Name, Parent, UPath, LocatorMessage);
        
        public bool IsRendering => Driver.Instance.IsRendering(Root, Name, Parent, UPath, LocatorMessage);
        
        public bool IsOnScreen => Driver.Instance.IsOnScreen(Root, Name, Parent, UPath, LocatorMessage);
        
        public bool IsGraphicClickable => Driver.Instance.IsGraphicClickable(Root, Name, Parent, UPath, LocatorMessage);
        
        public int Count => Driver.Instance.Count(Root, Name, Parent, UPath);

        public void Click()
        {
            Driver.Instance.Click(Root, Name, Parent, UPath, LocatorMessage);
        }

        public void SendKeys(string value)
        {
            Driver.Instance.SendKeys(value, Root, Name, Parent, UPath, LocatorMessage);
        }

        public void Swipe(Constants.Directions direction)
        {
            Driver.Instance.Swipe(Root, Name, Parent, UPath, direction.ToString().ToLower(), LocatorMessage);
        }

        public string GetComponent(string component)
        {
            return Driver.Instance.GetComponent(Root, Name, Parent, UPath, component, LocatorMessage);
        }
        
        public ScreenCoordinates GetCoordinates()
        {
            var result = Driver.Instance.GetCoordinates(Root, Name, Parent, UPath, LocatorMessage);

            return JsonConvert.DeserializeObject<ScreenCoordinates>(result);
        }

        public void Should(Condition condition, int timeoutMs)
        {
            Wait.For(() => condition.Invoke(this),
                () =>
                    $"Timed out after {timeoutMs / 1000} seconds while waiting for Condition: {condition}",
                Configuration.TimeoutMs);
        }

        public void Should(Condition condition)
        {
            Should(condition, Configuration.TimeoutMs);
        }

        public void ShouldNot(Condition condition, int timeoutMs)
        {
            Wait.For(() => !condition.Invoke(this),
                () =>
                    $"Timed out after {timeoutMs / 1000} seconds while waiting for Condition Not fulfilled: {condition}",
                Configuration.TimeoutMs);
        }

        public void ShouldNot(Condition condition)
        {
            ShouldNot(condition, Configuration.TimeoutMs);
        }
    }
}
