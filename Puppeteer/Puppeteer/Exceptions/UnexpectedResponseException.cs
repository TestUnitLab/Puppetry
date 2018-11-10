using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class UnexpectedResponseException : PuppetryException
    {
        public UnexpectedResponseException()
        {
        }

        public UnexpectedResponseException(string message) : base(message)
        {
        }

        public UnexpectedResponseException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
