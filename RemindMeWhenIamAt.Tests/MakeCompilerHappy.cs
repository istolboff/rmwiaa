using RemindMeWhenIamAt.Tests.Sut.Pages;

namespace RemindMeWhenIamAt.Tests
{
    public static class MakeCompilerHappy
    {
#pragma warning disable CA1801 // Review unused parameters
        public static void Use<T>(T unused)
#pragma warning restore CA1801 // Review unused parameters
        {
        }

        public static void UseClasses()
        {
            Use(new DefaultPage());
        }
    }
}
