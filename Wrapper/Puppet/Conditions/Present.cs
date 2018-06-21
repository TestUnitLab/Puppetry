namespace Puppet.Conditions
{
    internal class Present : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            _curentGameObject = gameObject;

            return gameObject.Exists();
        }

        protected override string DescribeExpected()
        {
            return $"Exists {true}";
        }

        protected override string DescribeActual()
        {
            return $"Exists {false}";
        }
    }

    public static partial class Be
    {
        public static Condition Existing => new Present();
    }
}