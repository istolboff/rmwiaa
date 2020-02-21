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
            var globalNugetPackagesFolder = Path.Combine(Environment.GetEnvironmentVariable("userprofile") ?? string.Empty, @".nuget\packages");
            var blazorDevserverDllPath = Path.Combine(globalNugetPackagesFolder, @$"microsoft.aspnetcore.blazor.devserver\{BlazorWebAssemblyTemplateVersion}\tools\blazor-devserver.dll");
            var pwaDllPath = Path.GetFullPath(Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, @"..\netstandard2.1\RemindMeWhenIamAt.dll"));
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = false,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "dotnet",
                Arguments = $"{blazorDevserverDllPath} serve --applicationpath {pwaDllPath}"
            };

            return Process.Start(startInfo);
        }

        private const string BlazorWebAssemblyTemplateVersion = "3.2.0-preview1.20073.1";
    }
}
