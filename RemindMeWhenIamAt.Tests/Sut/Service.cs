using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal static class Service
    {
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
                    FileName = Path.Combine(Directory.GetParent(typeof(Service).Assembly.Location).FullName, @"RemindMeWhenIamAt.Server.exe")
                });
        }
    }
}
