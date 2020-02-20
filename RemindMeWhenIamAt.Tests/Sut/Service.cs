using System.IO;
using System.Reflection;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal static class Service
    {
        public static string FullPath =>
            Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "RemindMeWhenIamAt.exe");
    }
}
