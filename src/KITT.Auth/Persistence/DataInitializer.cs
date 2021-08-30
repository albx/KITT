using KITT.Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace KITT.Auth.Persistence
{
    public class DataInitializer
    {
        private readonly DataInitializerOptions _options;

        private readonly UserManager<KittUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        const string ROLE = "Administrator";

        public DataInitializer(IOptions<DataInitializerOptions> options, UserManager<KittUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task InitializeAsync()
        {
            if (!(await _roleManager.RoleExistsAsync(ROLE)))
            {
                await _roleManager.CreateAsync(new IdentityRole(ROLE));
            }

            var user = await _userManager.FindByNameAsync(_options.UserName);
            if (user is null)
            {
                user = new KittUser
                {
                    UserName = _options.UserName,
                    Email = _options.Email,
                    TwitchChannel = _options.TwitchChannel,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, _options.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, ROLE);
                }
            }
        }

        public record DataInitializerOptions
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public string TwitchChannel { get; set; }

            public string Email { get; set; }
        }
    }
}
