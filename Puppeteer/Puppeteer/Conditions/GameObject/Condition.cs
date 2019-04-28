
namespace Puppetry.Puppeteer.Conditions.GameObject
{
    public abstract class Condition
    {
        protected Puppeteer.GameObject CurentGameObject;

        public abstract bool Invoke<T>(T gameObject) where T : Puppeteer.GameObject;

        protected abstract string DescribeExpected();

        protected abstract string DescribeActual();

        public override string ToString()
        {
            return $"\n{GetType().Name} for GameObject with {CurentGameObject.LocatorMessage} =>" +
                   $" \nExpected: {DescribeExpected()} \nActual: {DescribeActual()}";
        }
    }
}