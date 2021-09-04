using KITT.Auth.Models;
using LemonBot.Web.Models.Account;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace LemonBot.Web.Services
{
    public class AccountControllerServices
    {
        public SignInManager<KittUser> SignInManager { get; }

        public AccountControllerServices(SignInManager<KittUser> signInManager)
        {
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        public LoginViewModel GetLoginViewModel(string returnUrl)
        {
            var viewModel = new LoginViewModel { ReturnUrl = returnUrl };
            return viewModel;
        }

        public async Task<bool> SignInAsync(LoginViewModel model)
        {
            var result = await SignInManager.PasswordSignInAsync(
                model.UserName, 
                model.Password, 
                model.RememberLogin, 
                lockoutOnFailure: false);

            return result.Succeeded;
        }

        public Task SignOutAsync() => SignInManager.SignOutAsync();
    }
}
