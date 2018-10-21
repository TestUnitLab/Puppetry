using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class TimeoutException : PuppetDriverException
    {
        public TimeoutException()
        {
        }

        public TimeoutException(string message)
            : base(message)
        {
        }

        public TimeoutException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
