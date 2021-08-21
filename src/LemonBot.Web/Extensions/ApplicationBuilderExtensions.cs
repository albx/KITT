using Microsoft.AspNetCore.Builder;

namespace LemonBot.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Add the Console endpoint to the application
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseKITTConsole(this IApplicationBuilder app)
        {
            app.Map("/console", app =>
            {
                app.UseBlazorFrameworkFiles();
                app.UseRouting();

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
