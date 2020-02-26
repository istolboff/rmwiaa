using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RemindMeWhenIamAt.Tests.Sut
{
    internal sealed class ApplicationUnderTest
    {
        public static string RootUrl
        {
            get
            {
                var launchSettingsFilePath = Path.Combine(Service.FolderPath, "launchSettings.json");
                var urls = ((JsonElement)JsonSerializer.Deserialize<object>(File.ReadAllText(launchSettingsFilePath)))
                            .GetProperty("profiles")
                            .GetProperty("RemindMeWhenIamAt.Server")
                            .GetProperty("applicationUrl")
                            .ToString();
                return urls.Split(";").Single(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
            }
        }

        public TPage NavigateTo<TPage>()
            where TPage : class
        {
            throw new NotImplementedException();
        }
    }
}
