using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace RemindMeWhenIamAt.Client
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class Program
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync().ConfigureAwait(true);
        }
    }
}
