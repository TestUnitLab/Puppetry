namespace Puppetry.Puppeteer.Conditions
{  
    public class Screen : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            return gameObject.IsOnScreen;
        }

        protected override string DescribeExpected()
        {
            return "Is on the screen";
        }

        protected override string DescribeActual()
        {
            return "Is not on the screen";
        }
    }
    
    public static partial class Be
    {
        public static Condition OnScreen => new Screen();
    }
}