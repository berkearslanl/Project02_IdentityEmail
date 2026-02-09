using _2_EmailProject.Dtos;
using _2_EmailProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _2_EmailProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var values = await _userManager.FindByNameAsync(User.Identity.Name);
            UserEditDto userEditDto = new UserEditDto();
            userEditDto.Name = values.Name;
            userEditDto.Surname = values.Surname;
            userEditDto.Email = values.Email;
            userEditDto.ImageUrl = values.ImageUrl;
            return View(userEditDto);
        }
        [HttpPost]
        public async Task<IActionResult> Index(UserEditDto userEditDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            user.Name = userEditDto.Name;
            user.Surname = userEditDto.Surname;
            user.Email = userEditDto.Email;

            if (!string.IsNullOrEmpty(userEditDto.Password))
            {
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);
            }

            // RESİM SEÇİLMİŞ Mİ KONTROLÜ
            if (userEditDto.Image != null)
            {
                var resource = Directory.GetCurrentDirectory();
                var extension = Path.GetExtension(userEditDto.Image.FileName);
                var imageName = Guid.NewGuid() + extension;
                var saveLocation = Path.Combine(resource, "wwwroot/images", imageName);

                // using bloğu dosya işleminin bitince belleğin boşaltılmasını sağlar
                using (var stream = new FileStream(saveLocation, FileMode.Create))
                {
                    await userEditDto.Image.CopyToAsync(stream);
                }

                user.ImageUrl = imageName;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            return View(userEditDto);
        }
    }
}
