using FluentValidation;
using KITT.Auth;
using KITT.Auth.Models;
using KITT.Auth.Persistence;
using KITT.Core.Commands;
using KITT.Core.Persistence;
using KITT.Core.ReadModels;
using KITT.Core.Validators;
using LemonBot.Web.Extensions;
using LemonBot.Web.Hubs;
using LemonBot.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.AddDbContext<KittDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("KittDatabase")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services
                .AddDefaultIdentity<KittUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<KittIdentityDbContext>();

            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/Account/Login";
                options.UserInteraction.LogoutUrl = "/Account/Logout";
            }).AddApiAuthorization<KittUser, KittIdentityDbContext>();

            services
                .AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthDataInitializer(options =>
            {
                options.UserName = Configuration["AdministratorUser:UserName"];
                options.Password = Configuration["AdministratorUser:Password"];
                options.TwitchChannel = Configuration["AdministratorUser:TwitchChannel"];
                options.Email = Configuration["AdministratorUser:Email"];
            });

            services
                .AddValidatorsFromAssemblyContaining<StreamingValidator>()
                .AddScoped<IDatabase, Database>()
                .AddScoped<IStreamingCommands, StreamingCommands>();

            services.AddScoped<Areas.Console.Services.StreamingsControllerServices>();

            services.AddScoped<AccountControllerServices>();
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
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseKITTConsole(env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<BotMessageHub>("/bot");
            });
        }
    }
}
