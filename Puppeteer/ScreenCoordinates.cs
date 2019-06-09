using System;

namespace Puppetry.Puppeteer
{
    [Serializable]
    public class ScreenCoordinates
    {
        public float X { get; }
        public float Y { get; }

        public ScreenCoordinates(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}