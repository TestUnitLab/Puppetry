using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class TimeoutException : PuppetryException
    {
        public TimeoutException()
        {
        }

        public TimeoutException(string message) : base(message)
        {
        }

        public TimeoutException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
