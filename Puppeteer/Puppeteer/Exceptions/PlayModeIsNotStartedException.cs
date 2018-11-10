using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class PlayModeIsNotStartedException : PuppetryException
    {
        public PlayModeIsNotStartedException() : base("PlayMode is not started")
        {
        }

        public PlayModeIsNotStartedException(string message) : base(message)
        {
        }

        public PlayModeIsNotStartedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
