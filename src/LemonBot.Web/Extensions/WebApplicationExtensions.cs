using Hellang.Middleware.ProblemDetails;
using LemonBot.Web.GraphQL;
using LemonBot.Web.Hubs;

namespace LemonBot.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication Configure(this WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseProblemDetails();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<BotMessageHub>("/bot");
        app.MapKittGraphQL(env);

        app.MapFallbackToFile("index.html");

        return app;
    }
}
