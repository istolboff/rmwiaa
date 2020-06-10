using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                return new Uri(urls.Split(";").Single(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase)));
            }
        }

        public static Process Start()
        {
            var result = new Process();
            result.StartInfo.FileName = Path.Combine(FolderPath, @"RemindMeWhenIamAt.Server.exe");
            result.StartInfo.WorkingDirectory = Path.GetFullPath(Path.Combine(FolderPath, @"..\publish"));
            result.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            result.StartInfo.RedirectStandardOutput = true;
            result.OutputDataReceived += (_, e) => serverOutput.Enqueue(e.Data + Environment.NewLine);
            result.Start();
            result.BeginOutputReadLine();
            return result;
        }

        public static string ReadOutput()
        {
            var result = new StringBuilder();
            while (serverOutput.TryDequeue(out var nextPortion))
            {
                result.Append(nextPortion);
            }

            return result.ToString();
        }

        private static string FolderPath => Directory.GetParent(typeof(Startup).Assembly.Location).FullName;

        private static ConcurrentQueue<string> serverOutput = new ConcurrentQueue<string>();
    }
}
