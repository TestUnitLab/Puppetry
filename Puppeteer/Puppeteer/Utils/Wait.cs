using System;
using System.Diagnostics;
using System.Threading;

namespace Puppeteer.Utils
{
    internal static class Wait
    {
        public static void For(Func<bool> condition, string errorMessage, int waitTimeout)
        {
            if (!TryToWaitFor(condition, waitTimeout))
            {
                throw new Exception(errorMessage);
            }
        }

        public static void For(Func<bool> condition, Func<string> errorMessage, int waitTimeout)
        {
            if (!TryToWaitFor(condition, waitTimeout))
            {
                throw new Exception(errorMessage.Invoke());
            }
        }

        private static bool TryToWaitFor(Func<bool> condition, int waitTimeout)
        {
            var isSuccess = false;
            var alreadyWaited = 0;
            var timeToWait = 0;
            Stopwatch stopwatch = new Stopwatch();

            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();
                var result = condition.Invoke();
                if (result)
                {
                    isSuccess = true;
                    break;
                }

                stopwatch.Stop();

                alreadyWaited += stopwatch.Elapsed.Milliseconds;

                if (alreadyWaited >= waitTimeout)
                    break;

                if (timeToWait == 0) timeToWait += 100;
                else timeToWait *= 2;

                Thread.Sleep(timeToWait);

                alreadyWaited += timeToWait;
            }

            return isSuccess;
        }
    }
}