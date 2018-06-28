using System;

namespace Puppeteer.Exceptions
{
    public class PuppetDriverException : Exception
    {
        public PuppetDriverException()
        {
        }

        public PuppetDriverException(string message)
            : base(message)
        {
        }

        public PuppetDriverException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
