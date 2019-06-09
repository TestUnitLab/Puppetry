using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class NoSuchGameObjectException : PuppetryException
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
