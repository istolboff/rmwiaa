using System.Diagnostics;
using System.IO;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal static class Service
    {
        public static string FolderPath => Directory.GetParent(typeof(Service).Assembly.Location).FullName;

        public static Process Start()
        {
            return Process.Start(
                new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = Path.Combine(FolderPath, @"RemindMeWhenIamAt.Server.exe"),
                    WorkingDirectory = Path.Combine(FolderPath, @"..\..\publish")
                });
        }
    }
}
