using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class PuppetryException : Exception
    {
        public PuppetryException()
        {
        }

        public PuppetryException(string message) : base(message)
        {
        }

        public PuppetryException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
