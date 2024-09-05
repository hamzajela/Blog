using Blog.Web.Models.ViewModels;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminUsersController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly UserManager<IdentityUser> userManager;

        public AdminUsersController(IUserRepository userRepository,
            UserManager<IdentityUser> userManager) {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        [HttpGet]
        public async  Task<IActionResult> List()
        {
            var users = await userRepository.GetAll();
            var usersViewModel = new UserViewModel();
            
            usersViewModel.Users=new List<User>();
            
            foreach(var user in users) 
            {
                usersViewModel.Users.Add(
                    new Models.ViewModels.User
                    {
                        Id = Guid.Parse(user.Id),
                        UserName = user.UserName,
                        EmailAddress = user.Email
,
                    });


            }

            return View(usersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> List(UserViewModel reuqest) 
        {
            var identityUser = new IdentityUser
            {
                UserName = reuqest.Username,
                Email = reuqest.Email

            };
          var identityResult=  await userManager.CreateAsync(identityUser, reuqest.Password);
           
            if(identityResult is not null) 
            {
                if (identityResult.Succeeded) 
                {
                    //Assign roles to this user
                    // every user created gets role 'User' by default
                     var roles=new List<string> { "User"};
                    if (reuqest.AdminRoleCheckbox)
                    {
                        roles.Add("Admin");

                    }
                   identityResult= await userManager.AddToRolesAsync(identityUser, roles);
               
                  if (identityResult is not null && identityResult.Succeeded)
                    {
                        return RedirectToAction("List", "AdminUsers");   
                    }
                
                }


            }
           return View();
        
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user=await userManager.FindByIdAsync(id.ToString()); 
             if(user != null) 
            {
              var identityResult =  await userManager.DeleteAsync(user);
               if (identityResult != null && identityResult.Succeeded) 
                {
                    return RedirectToAction("List", "AdminUsers");
                }
            
            }

            return View();
        }
    }
}
