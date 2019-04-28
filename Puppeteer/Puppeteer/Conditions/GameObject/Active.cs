namespace Puppetry.Puppeteer.Conditions.GameObject
{
    internal class Active : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            return gameObject.IsActiveInHierarchy;
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
        public static Condition ActiveInHierarchy => new Active();
    }
}