namespace Puppetry.Puppeteer.Conditions
{
    public class GraphicClick : Condition
    {
        public override bool Invoke<T>(T gameObject)
        {
            CurentGameObject = gameObject;
            return gameObject.IsGraphicClickable;
        }

        protected override string DescribeExpected()
        {
            return "GameObject is UI Graphic clicked";
        }

        protected override string DescribeActual()
        {
            return "GameObject is not UI Graphic clickable";
        }
    }
    
    public static partial class Be
    {
        public static Condition GraphicClickable => new GraphicClick();
    }
}