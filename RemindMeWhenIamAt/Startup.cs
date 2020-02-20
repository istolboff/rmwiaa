using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RemindMeWhenIamAt
{
    public class Startup
    {
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1801 // Review unused parameters
        public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1801 // Review unused parameters
#pragma warning restore CA1822 // Mark members as static
        {
        }

#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!").ConfigureAwait(true);
                });
            });
        }
    }
}
