namespace Puppetry.Puppeteer.Conditions
{
    internal class Component : Condition
    {
        private string component;

        public Component(string componentName)
        {
            component = componentName;
        }

        public override bool Invoke<T>(T gameObject)
        {
            _curentGameObject = gameObject;
            return gameObject.GetComponent(component) != null;
        }

        protected override string DescribeExpected()
        {
            return $"Has component {component}";
        }

        protected override string DescribeActual()
        {
            return "But does not have it";
        }
    }

    public static partial class Have
    {
        public static Condition Component(string component)
        {
            return new Component(component);
        }
    }
}
