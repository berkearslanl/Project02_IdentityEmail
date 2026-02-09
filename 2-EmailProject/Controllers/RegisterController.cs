using _2_EmailProject.Dtos;
using _2_EmailProject.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace _2_EmailProject.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRegisterDto userRegisterDto)
        {
            string code = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            AppUser appUser = new AppUser
            {
                Name = userRegisterDto.Name,
                Surname = userRegisterDto.Surname,
                UserName = userRegisterDto.Username,
                Email = userRegisterDto.Email,
                ConfirmCode = code,
                EmailConfirmed = false
            };
            var result = await _userManager.CreateAsync(appUser, userRegisterDto.Password);

            if (result.Succeeded)
            {
                var mimeMessage = new MimeMessage();

                MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "berkesude39@gmail.com");
                mimeMessage.From.Add(mailboxAddressFrom);

                MailboxAddress mailboxAddressTo = new MailboxAddress("User", appUser.Email);
                mimeMessage.To.Add(mailboxAddressTo);

                mimeMessage.Subject = "Email Doğrulama Kodu";

                var bodyBuilder = new BodyBuilder()
                {
                    HtmlBody = $"Email doğrulama kodunuz : {code}"
                };
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                using var smtpClient = new SmtpClient();
                smtpClient.Connect("smtp.gmail.com", 587, false);
                smtpClient.Authenticate("berkesude39@gmail.com", "ttym seem mhda yxxp");
                smtpClient.Send(mimeMessage);
                smtpClient.Disconnect(true);
                TempData["KayıtBaşarılı"] = "Kayıt işlemi başarılı. Giriş yapma sayfasına yönlendiriliyorsunuz!";
                return RedirectToAction("UserLogin","Login");
            }
            else
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail()
        {
            TempData["EmailDoğrulama"] = "Lütfen e-posta doğrulama işlemini tamamlayın!";
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("UserLogin", "Login");
            }
            ViewBag.email = user.Email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(string code)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user.ConfirmCode == code)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                TempData["Başarılı"] = "Doğrulama işlemi başarılı. Giriş yapma sayfasına yönlendiriliyorsunuz!";
                return RedirectToAction("UserLogin", "Login");
            }
            ViewBag.email = user.Email;
            TempData["KodHatası"] = "Doğrulama kodu hatalı! Lütfen tekrar deneyiniz.";
            return View();
        }

    }
}
