using System.Linq;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class IntExtensions
    {
        public static string Times(this int @this, string value)
        {
            return string.Join(string.Empty, Enumerable.Repeat(value, @this));
        }
    }
}