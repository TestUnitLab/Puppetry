using Puppeteer.Puppet;

namespace Puppeteer.Conditions
{
    public abstract class Condition
    {
        protected GameObject _curentGameObject;

        public abstract bool Invoke<T>(T gameObject) where T : GameObject;

        protected abstract string DescribeExpected();

        protected abstract string DescribeActual();

        public override string ToString()
        {
            return $"\n{GetType().Name} for GameObject with name: {_curentGameObject.Name} and parent: {_curentGameObject.Parent ?? "null"} =>" +
                   $" \nExpected: {DescribeExpected()} \nActual: {DescribeActual()}";
        }
    }
}