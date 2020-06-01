using System;
using JetBrains.Annotations;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class Verify
    {
        [AssertionMethod]
        public static void That(bool condition, ExceptionMessageFactory exceptionMessageFactory)
        {
            if (!condition)
            {
                throw new InvalidOperationException(exceptionMessageFactory.CreateMessage());
            }
        }
    }
}