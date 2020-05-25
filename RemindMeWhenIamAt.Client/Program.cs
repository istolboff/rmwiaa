using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

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

#pragma warning disable CA2000 // Container takes care of disposing HttpClient
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
#pragma warning restore CA2000 // DisposeObjectsBeforeLosingScope

            await builder.Build().RunAsync().ConfigureAwait(true);
        }
    }
}
