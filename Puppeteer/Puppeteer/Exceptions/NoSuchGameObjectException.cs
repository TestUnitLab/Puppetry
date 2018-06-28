using System;

namespace Puppeteer.Exceptions
{
    public class NoSuchGameObjectException : PuppetDriverException
    {
        public NoSuchGameObjectException()
        {
        }

        public NoSuchGameObjectException(string message)
            : base(message)
        {
        }

        public NoSuchGameObjectException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
