using System;
using System.Diagnostics;
using RemindMeWhenIamAt.Tests.Miscellaneous;
using static PwaHomeScreenUtilities.ExceptionFilters;

namespace PwaHomeScreenUtilities
{
    internal static class FuncExtensions
    {
        public static Func<TArg, TResult> TraceExceptions<TArg, TResult>(this Func<TArg, TResult> @this) =>
            arg =>
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