using System;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class ExceptionFilters
    {
        public static bool True(Action action)
        {
            action();
            return true;
        }
    }
}