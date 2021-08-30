using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace LemonBot.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the Console endpoint to the application
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKITTConsole(this IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            app.Map("/console", app =>
            {
                if (env.IsDevelopment())
                {
                    app.UseWebAssemblyDebugging();
                }

                app.UseBlazorFrameworkFiles();
                app.UseRouting();

                //app.UseIdentityServer();
                //app.UseAuthentication();
                //app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToFile("console/index.html");
                });
            });

            return app;
        }
    }
}
