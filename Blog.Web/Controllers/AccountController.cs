using Blog.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userMananger;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userMananger,
           SignInManager<IdentityUser> signInManager )
        {
            this.userMananger = userMananger;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel) 
        {
           
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = registerViewModel.Username,
                    Email = registerViewModel.Email
                };
                var identityResult = await userMananger.CreateAsync(identityUser, registerViewModel.Password);

                if (identityResult.Succeeded)
                {
                    // Assign this user the "User" role
                    var roleIdentityResult = await userMananger.AddToRoleAsync(identityUser, "User");

                    if (roleIdentityResult.Succeeded)
                    {

                        // Show success notification
                        return RedirectToAction("Register");
                    }
                }

            }
        

            // Show error notification
            return View();

        }


        [HttpGet]
        public IActionResult Login( string ReturnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = ReturnUrl
            };
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
           if (!ModelState.IsValid)
            {
                return View();
            }
            
            var signIn=await signInManager.PasswordSignInAsync(loginViewModel.Username, loginViewModel.Password, false, false);
            if (signIn!=null && signIn.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(loginViewModel.ReturnUrl))
                {
                    return Redirect(loginViewModel.ReturnUrl);    
   
                }
                return RedirectToAction("Index", "Home");
            }

            return View();

        }

        [HttpGet]
        public async Task<IActionResult>Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() 
        {
            return View(); 
        }
    }
}
