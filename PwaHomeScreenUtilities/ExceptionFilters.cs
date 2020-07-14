using System;

namespace PwaHomeScreenUtilities
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