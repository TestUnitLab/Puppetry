using System;

namespace Puppeteer.Exceptions
{
    public class SessionCreationException : PuppetDriverException
    {
        public SessionCreationException()
        {
        }

        public SessionCreationException(string message)
            : base(message)
        {
        }

        public SessionCreationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
