using Hellang.Middleware.ProblemDetails;
using LemonBot.Web.Endpoints;
using LemonBot.Web.GraphQL;

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

        app.MapStreamings();

        app.MapKittGraphQL(env);

        app.MapFallbackToFile("index.html");

        return app;
    }
}
