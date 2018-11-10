using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class SessionCreationException : PuppetryException
    {
        public SessionCreationException()
        {
        }

        public SessionCreationException(string message) : base(message)
        {
        }

        public SessionCreationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
