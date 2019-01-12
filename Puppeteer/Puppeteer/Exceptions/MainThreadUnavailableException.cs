using System;

namespace Puppetry.Puppeteer.Exceptions
{
    public class MainThreadUnavailableException : PuppetryException
    {
        public MainThreadUnavailableException() : base("Main Thread is unavailable or overloaded")
        {
        }

        public MainThreadUnavailableException(string message) : base(message)
        {
        }

        public MainThreadUnavailableException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
