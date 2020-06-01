using System;
using JetBrains.Annotations;
using RemindMeWhenIamAt.Tests.StepDefinitions;
using RemindMeWhenIamAt.Tests.Sut;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class MakeCompilerHappy
    {
#pragma warning disable CA1801 // Review unused parameters
        public static void Use<T>([UsedImplicitly]T unused)
#pragma warning restore CA1801 // Review unused parameters
        {
        }

// ReSharper disable UnusedMember.Global
        public static void GetRidOfCa1812()
// ReSharper enable UnusedMember.Global
        {
            Use(new object[] { new ApplicationUnderTest(null!, null!), new TestRun(null!), new BasicActions(null!) });
        }

        public static T EnsureNotNull<T>(T? v)
            where T : class
        {
            return v ?? throw new ArgumentNullException(nameof(v));
        }

        public static string SuppressCa1303(string value) => value;

        public static Func<T> D<T>(Func<T> v) => v;
    }
}
