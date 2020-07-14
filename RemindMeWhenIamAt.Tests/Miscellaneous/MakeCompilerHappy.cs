using System;
using JetBrains.Annotations;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class MakeCompilerHappy
    {
#pragma warning disable CA1801 // Review unused parameters
        public static void Use<T>([UsedImplicitly]T unused)
#pragma warning restore CA1801 // Review unused parameters
        {
        }

        public static T EnsureNotNull<T>(T? v)
            where T : class
        {
            return v ?? throw new ArgumentNullException(nameof(v));
        }

        public static string SuppressCa1303(string value) => value;

        public static void NoOp()
        {
        }
    }
}
