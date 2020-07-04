using System;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal static class UriExtensions
    {
        public static string CutSchemeOff(this Uri @this) =>
            @this.ToString().Remove(0, @this.Scheme.Length + "://".Length);
    }
}