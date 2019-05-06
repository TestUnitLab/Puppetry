using Puppetry.Puppeteer.Constants;

namespace Puppetry.Puppeteer.Conditions.GameObject
{
    public class Raycast : Condition
    {
        private Raycasters _raycater;

        public Raycast(Raycasters raycaster)
        {
            _raycater = raycaster;
        }

        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            return gameObject.IsHitByRaycast(_raycater);
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
        public static Condition HitByRaycast(Raycasters raycaster)
        {
            return new Raycast(raycaster);
        }
    }
}