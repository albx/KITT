//using LemonBot.Web.Models.Account;
//using LemonBot.Web.Services;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;

//namespace LemonBot.Web.Controllers
//{
//    public class AccountController : Controller
//    {
//        public AccountControllerServices ControllerServices { get; }

//        public AccountController(AccountControllerServices controllerServices)
//        {
//            ControllerServices = controllerServices ?? throw new ArgumentNullException(nameof(controllerServices));
//        }

//        [HttpGet]
//        public IActionResult Login(string returnUrl)
//        {
//            var viewModel = ControllerServices.GetLoginViewModel(returnUrl);
//            return View(viewModel);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(LoginViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                var userSignedIn = await ControllerServices.SignInAsync(model);
//                if (userSignedIn)
//                {
//                    if (Url.IsLocalUrl(model.ReturnUrl))
//                    {
//                        return Redirect(model.ReturnUrl);
//                    }

//                    return RedirectToHome();
//                }
//            }

//            return View(model);
//        }

//        [HttpGet]
//        public async Task<IActionResult> Logout(string logoutId)
//        {
//            await ControllerServices.SignOutAsync();
//            return RedirectToHome();
//        }

//        private IActionResult RedirectToHome() => RedirectToAction("Index", "Home");
//    }
//}
