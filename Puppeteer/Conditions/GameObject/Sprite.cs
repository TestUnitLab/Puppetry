
namespace Puppetry.Puppeteer.Conditions.GameObject
{
    public class Sprite : Condition
    {
        private string _expectedSpriteName;
        private string _actualSpriteName;

        public Sprite(string spriteName)
        {
            _expectedSpriteName = spriteName;
        }

        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            _actualSpriteName = CurentGameObject.SpriteName;

            return _actualSpriteName == _expectedSpriteName;
        }

        protected override string DescribeExpected()
        {
            return $"Has sprite with name {_expectedSpriteName}";
        }

        protected override string DescribeActual()
        {
            return $"Has sprite with name {_actualSpriteName}";
        }
    }

    public static partial class Have
    {
        public static Condition SpriteName(string spriteName)
        {
            return new Sprite(spriteName);
        }
    }
}
