using System;

namespace Puppeteer.Exceptions
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
