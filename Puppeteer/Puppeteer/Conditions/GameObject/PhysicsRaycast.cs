namespace Puppetry.Puppeteer.Conditions.GameObject
{
    public class PhysicsRaycast : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            return gameObject.IsHitByPhysicsRaycast;
        }

        protected override string DescribeExpected()
        {
            return "GameObject is hit by PhysicsRaycast";
        }

        protected override string DescribeActual()
        {
            return "GameObject is not hit by PhysicsRaycast";
        }
    }
    
    public static partial class Be
    {
        public static Condition HitByPhysicsRaycast => new PhysicsRaycast();
    }
}