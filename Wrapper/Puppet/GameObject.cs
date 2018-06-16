using System;

namespace Puppet
{
    public class GameObject
    {
        private string _name;
        private string _parent;

        public GameObject(string name)
        {
            _name = name;
        }

        public GameObject(string name, string parent)
        {
            _name = name;
            _parent = parent;
        }

        public void Click()
        {
            throw new NotImplementedException();
        }

        public void SendKeys()
        {
            throw new NotImplementedException();
        }

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public bool IsDisplayed()
        {
            throw new NotImplementedException();
        }
    }
}
