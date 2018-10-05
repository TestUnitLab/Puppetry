namespace Puppetry.Puppeteer.Conditions
{
    internal class Rendering : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            _curentGameObject = gameObject;
            return gameObject.IsRendering;
        }

        protected override string DescribeExpected()
        {
            return $"Is Rendering {true}";
        }

        protected override string DescribeActual()
        {
            return $"Is Rendering {false}";
        }
    }

    public static partial class Be
    {
        public static Condition Rendered => new Rendering();
    }
}