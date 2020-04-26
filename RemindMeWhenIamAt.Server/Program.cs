using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace RemindMeWhenIamAt.Server
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public static class Program
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
