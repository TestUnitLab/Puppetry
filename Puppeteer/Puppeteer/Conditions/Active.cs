namespace Puppetry.Puppeteer.Conditions
{
    internal class Active : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            _curentGameObject = gameObject;
            return gameObject.IsActive();
        }

        protected override string DescribeExpected()
        {
            return $"Active {true}";
        }

        protected override string DescribeActual()
        {
            return $"Active {false}";
        }
    }

    public static partial class Be
    {
        public static Condition Active => new Active();
    }
}