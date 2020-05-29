using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class Wait
    {
        public static void Until(Func<bool> predicate, TimeSpan timeout, [CallerMemberName] string? callerMemberName = null)
        {
            Until(predicate, value => value, timeout, null, callerMemberName);
        }

        public static T Until<T>(
            Func<T> selectValue,
            Predicate<T> checkValue,
            TimeSpan timeout,
            Func<T, Exception>? makeInnerException,
            [CallerMemberName] string? callerMemberName = null)
        {
            var stopWatch = Stopwatch.StartNew();

            while (true)
            {
                var value = selectValue();
                if (checkValue(value))
                {
                    return value;
                }

                if (stopWatch.Elapsed >= timeout)
                {
#pragma warning disable CA1303
                    throw makeInnerException == null
                        ? new TimeoutException("Waiting failed.")
                        : new TimeoutException("Waiting failed.", makeInnerException(value));
#pragma warning restore CA1303
                }

                Trace.WriteLine($"Probing in Wait.Until() called from {callerMemberName} failed, sleeping for {AWhile}.");
                Thread.Sleep(AWhile);
            }
        }

        private static readonly TimeSpan AWhile = TimeSpan.FromMilliseconds(250);
    }
}