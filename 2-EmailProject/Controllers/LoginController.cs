using _2_EmailProject.Context;
using _2_EmailProject.Dtos;
using _2_EmailProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace _2_EmailProject.Controllers
{
    public class LoginController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginDto userLoginDto)
        {
            
            var user = await _userManager.FindByNameAsync(userLoginDto.Username);
            if (user == null)
            {
                TempData["HatalıGiriş"] = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(userLoginDto.Username, userLoginDto.Password, true, false);

            if (result.Succeeded)
            {
                if (user.EmailConfirmed == true)
                {
                    return RedirectToAction("Index", "Mailbox");
                }
                else
                {
                    return RedirectToAction("ConfirmEmail", "Register");
                }
            }
            else 
            {
                TempData["HatalıGiriş"] = "E-posta adresi veya şifre hatalı ! Lütfen tekrar deneyiniz.";
                return View();
            }
            
                
        }
    }
}
