using System;
using System.Diagnostics;
using System.Threading;

namespace Puppetry.Puppeteer.Utils
{
    internal static class Wait
    {
        public static void For(Func<bool> condition, string errorMessage, int waitTimeout)
        {
            if (!TryToWaitFor(condition, waitTimeout))
            {
                throw new Exceptions.TimeoutException(errorMessage);
            }
        }

        public static void For(Func<bool> condition, Func<string> errorMessage, int waitTimeout)
        {
            if (!TryToWaitFor(condition, waitTimeout))
            {
                throw new Exceptions.TimeoutException(errorMessage.Invoke());
            }
        }

        private static bool TryToWaitFor(Func<bool> condition, int waitTimeout)
        {
            var isSuccess = false;
            var alreadyWaited = 0;
            var timeToWait = Configuration.PollingStratagy == PollingStrategies.Progressive ? 0 : 500;
            var stopwatch = new Stopwatch();

            while (true)
            {
                stopwatch.Reset();
                stopwatch.Start();
                bool result;
                try
                {
                    result = condition.Invoke();
                }
                catch (Exception)
                {
                    result = false;
                }
                
                if (result)
                {
                    isSuccess = true;
                    break;
                }

                stopwatch.Stop();

                alreadyWaited += stopwatch.Elapsed.Milliseconds;

                if (alreadyWaited >= waitTimeout)
                    break;
                
                if (Configuration.PollingStratagy == PollingStrategies.Progressive)
                {
                    if (timeToWait == 0) timeToWait += 100;
                    else timeToWait *= 2;
                }

                Thread.Sleep(timeToWait);

                alreadyWaited += timeToWait;
            }

            return isSuccess;
        }
    }
}