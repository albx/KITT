using FluentValidation;
using KITT.Core.Commands;
using KITT.Core.Persistence;
using KITT.Core.ReadModels;
using KITT.Core.Validators;
using LemonBot.Web.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

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
            //services.AddDbContext<KittDbContext>(
            //    options => options.UseSqlServer(Configuration.GetConnectionString("KittDatabase")));

            services.AddDbContext<KittDbContext>(
                options => options.UseInMemoryDatabase("Kitt-InMemory"));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.Configure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters.NameClaimType = "name";
                });

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAdB2C"));

            services
                .AddValidatorsFromAssemblyContaining<StreamingValidator>()
                .AddScoped<IDatabase, Database>()
                .AddScoped<ISettingsCommands, SettingsCommands>()
                .AddScoped<IStreamingCommands, StreamingCommands>();

            services
                .AddScoped<Areas.Console.Services.StreamingsControllerServices>()
                .AddScoped<Areas.Console.Services.SettingsControllerServices>();

            //services.AddScoped<AccountControllerServices>();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<BotMessageHub>("/bot");

                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
