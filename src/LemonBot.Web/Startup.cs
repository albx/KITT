using KITT.Auth.Models;
using KITT.Auth.Persistence;
using LemonBot.Web.Extensions;
using LemonBot.Web.Hubs;
using LemonBot.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace LemonBot.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<KittIdentityDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("KittDatabase")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<KittUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<KittIdentityDbContext>();

            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
            }).AddApiAuthorization<KittUser, KittIdentityDbContext>();

            services
                .AddAuthentication()
                .AddIdentityServerJwt();

            services.AddScoped<StreamingsControllerServices>();

            services.AddSignalR();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/console", ctx =>
                {
                    endpoints.MapFallbackToFile("console/index.html");
                    return Task.CompletedTask;
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<BotMessageHub>("/bot");
            });

            //app.UseKITTConsole();
        }
    }
}
