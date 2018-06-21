using Puppet.Conditions;

namespace Puppet
{
    public class GameObject
    {
        internal string Name;
        internal string Parent;

        public GameObject(string name)
        {
            Name = name;
        }

        public GameObject(string name, string parent)
        {
            Name = name;
            Parent = parent;
        }

        public void Click()
        {
            Driver.Instance.Click(Name, Parent);
        }

        public void SendKeys(string value)
        {
            Driver.Instance.SendKeys(value, Name, Parent);
        }

        public bool Exists()
        {
            return Driver.Instance.Exist(Name, Parent);
        }

        public bool IsActive()
        {
            return Driver.Instance.Active(Name, Parent);
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
