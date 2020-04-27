using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using RemindMeWhenIamAt.Server;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal static class Service
    {
        public static Uri RootUrl
        {
            get
            {
                var launchSettingsFilePath = Path.Combine(Service.FolderPath, @"Properties\launchSettings.json");
                var urls = ((JsonElement)JsonSerializer.Deserialize<object>(File.ReadAllText(launchSettingsFilePath)))
                            .GetProperty("profiles")
                            .GetProperty("RemindMeWhenIamAt.Server")
                            .GetProperty("applicationUrl")
                            .ToString();
                return new Uri(urls.Split(";").Single(u => u.StartsWith("http://", StringComparison.OrdinalIgnoreCase)));
            }
        }

        public static Process Start()
        {
            return Process.Start(
                new ProcessStartInfo
                {
                    FileName = Path.Combine(FolderPath, @"RemindMeWhenIamAt.Server.exe"),
                    Arguments = "--httpsredirection=off",
                    WorkingDirectory = Path.GetFullPath(Path.Combine(FolderPath, @"..\publish")),
                    WindowStyle = ProcessWindowStyle.Normal
                });
        }

        private static string FolderPath => Directory.GetParent(typeof(Startup).Assembly.Location).FullName;
    }
}
