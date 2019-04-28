
namespace Puppetry.Puppeteer.Conditions.Game
{
    internal class Scene : Condition
    {
        private string _expectedName;
        private string _acturalName;

        internal Scene(string sceneName)
        {
            _expectedName = sceneName;
        }

        public override bool Invoke()
        {
            _acturalName = Puppeteer.Game.GetSceneName();
            return _acturalName == _expectedName;
        }

        protected override string DescribeActual()
        {
            return $"Actual scene name is {_acturalName}";
        }

        protected override string DescribeExpected()
        {
            return $"Expected scene name is {_expectedName}";
        }
    }

    public static partial class Have
    {
        public static Condition SceneName(string sceneName)
        {
            return new Scene(sceneName);
        }
    }
}
