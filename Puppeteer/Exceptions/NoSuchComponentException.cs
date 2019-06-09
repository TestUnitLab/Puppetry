using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class NoSuchComponentException : PuppetryException
    {
        public NoSuchComponentException()
        {
        }

        public NoSuchComponentException(string message)
            : base(message)
        {
        }

        public NoSuchComponentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
