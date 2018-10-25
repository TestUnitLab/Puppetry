namespace Puppetry.Puppeteer.Conditions
{
    public abstract class Condition
    {
        protected GameObject CurentGameObject;

        public abstract bool Invoke<T>(T gameObject) where T : GameObject;

        protected abstract string DescribeExpected();

        protected abstract string DescribeActual();

        public override string ToString()
        {
            return $"\n{GetType().Name} for GameObject with {CurentGameObject.LocatorMessage} =>" +
                   $" \nExpected: {DescribeExpected()} \nActual: {DescribeActual()}";
        }
    }
}