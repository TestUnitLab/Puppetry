using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class MethodIsNotSupportedException  : PuppetryException
    {
        public MethodIsNotSupportedException()
        {
        }

        public MethodIsNotSupportedException(string message)
            : base(message)
        {
        }

        public MethodIsNotSupportedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}