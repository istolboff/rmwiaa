using System;
using System.Diagnostics;
using static RemindMeWhenIamAt.Tests.Miscellaneous.ExceptionFilters;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class FuncExtensions
    {
        public static Func<TArg, TResult> TraceExceptions<TArg, TResult>(this Func<TArg, TResult> @this) =>
            (TArg arg) =>
            {
                try
                {
                    return @this(arg);
                }
                catch (Exception exception) when (!exception.ShouldNotBeCaught() && True(() => Trace.WriteLine(exception)))
                {
                    throw;
                }
            };
    }
}