using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
                    ErrorDialog = false,
                    LoadUserProfile = true,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = Path.Combine(FolderPath, @"RemindMeWhenIamAt.Server.exe")
                });
        }
    }
}
