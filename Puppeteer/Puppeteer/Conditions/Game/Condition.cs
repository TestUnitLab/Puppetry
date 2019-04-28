
namespace Puppetry.Puppeteer.Conditions.Game
{
    public abstract class Condition
    {
        public abstract bool Invoke();

        protected abstract string DescribeExpected();

        protected abstract string DescribeActual();

        public override string ToString()
        {
            return $"\n{GetType().Name} for the Game =>" +
                   $" \nExpected: {DescribeExpected()} \nActual: {DescribeActual()}";
        }
    }
}