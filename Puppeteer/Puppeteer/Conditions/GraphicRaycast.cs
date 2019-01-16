namespace Puppetry.Puppeteer.Conditions
{
    public class GraphicRaycast : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            return gameObject.IsHitByGraphicRaycast;
        }

        protected override string DescribeExpected()
        {
            return "GameObject is hit by GraphicRaycast";
        }

        protected override string DescribeActual()
        {
            return "GameObject is not hit by GraphicRaycast";
        }
    }
    
    public static partial class Be
    {
        public static Condition HitByGraphicRaycast => new GraphicRaycast();
    }
}