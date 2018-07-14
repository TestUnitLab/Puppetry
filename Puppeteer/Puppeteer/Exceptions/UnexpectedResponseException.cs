using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class UnexpectedResponseException : PuppetDriverException
    {
        public UnexpectedResponseException()
        {
        }

        public UnexpectedResponseException(string message)
            : base(message)
        {
        }

        public UnexpectedResponseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
