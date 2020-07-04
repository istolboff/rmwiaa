using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;

using static RemindMeWhenIamAt.Tests.Miscellaneous.MakeCompilerHappy;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class Wait
    {
        public static void Until(
            Expression<Func<bool>> predicate,
            TimeSpan? timeout = default)
        {
            Until(predicate, _ => _, timeout ?? TimeSpan.MaxValue);

            if (predicate.ToString() == "() => Debugger.IsAttached")
            {
                NoOp();
            }
        }

        public static T Until<T>(
            Expression<Func<T>> selectValueExpression,
            Expression<Predicate<T>> checkValueExpression,
            TimeSpan timeout,
            Func<T, Exception>? makeInnerException = null,
            string? waitingForWhat = default)
        {
            var stopWatch = Stopwatch.StartNew();

            if (!string.IsNullOrWhiteSpace(waitingForWhat))
            {
                Trace.WriteLine($"Started waiting for {waitingForWhat}");
            }

            var selectValue = selectValueExpression.Compile();
            var checkValue = checkValueExpression.Compile();

            while (true)
            {
                var value = selectValue();
                if (checkValue(value))
                {
                    return value;
                }

                if (stopWatch.Elapsed >= timeout)
                {
                    throw makeInnerException == null
                        ? new TimeoutException(SuppressCa1303("Waiting failed."))
                        : new TimeoutException(SuppressCa1303("Waiting failed."), makeInnerException(value));
                }

                if (!string.IsNullOrWhiteSpace(waitingForWhat))
                {
                    Trace.WriteLine($"Will check for {waitingForWhat} again in {AWhile}");
                }

                Thread.Sleep(AWhile);
            }
        }

        private static readonly TimeSpan AWhile = TimeSpan.FromMilliseconds(250);
    }
}