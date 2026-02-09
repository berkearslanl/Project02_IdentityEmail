using _2_EmailProject.Context;
using _2_EmailProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _2_EmailProject.Controllers
{
    public class MailboxController : Controller
    {
        private readonly EmailContext _emailContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinmanager;

        public MailboxController(EmailContext emailContext, UserManager<AppUser> userManager, SignInManager<AppUser> signinmanager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
            _signinmanager = signinmanager;
        }

        public async Task<IActionResult> Index(string folder = "inbox")
        {
            ViewBag.ActiveFolder = folder.ToLower();

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.user = user.Name + " " + user.Surname;
            ViewBag.username = user.UserName;
            ViewBag.image = user.ImageUrl;

            ViewBag.okunmuş = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStatus == true && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.okunmamış = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStatus == false && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.taslak = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsDraft == true && x.IsDeleted==false).Count();
            ViewBag.yıldızlı = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStars == true && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.çöp = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsDeleted == true).Count();
            ViewBag.mesaj = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsDraft==false && x.IsDeleted==false).Count();
            ViewBag.gönderilenmesaj = _emailContext.Messages.Where(x => x.SenderEmail == user.Email && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.iş = _emailContext.Messages.Where(x => x.CategoryId == 1 && x.ReceiverEmail==user.Email && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.eğitim = _emailContext.Messages.Where(x => x.CategoryId == 2 && x.ReceiverEmail == user.Email && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.aile = _emailContext.Messages.Where(x => x.CategoryId == 3 && x.ReceiverEmail == user.Email && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.sosyal = _emailContext.Messages.Where(x => x.CategoryId == 4 && x.ReceiverEmail == user.Email && x.IsDeleted == false && x.IsDraft == false).Count();
            ViewBag.diğer = _emailContext.Messages.Where(x => x.CategoryId == 5 && x.ReceiverEmail == user.Email && x.IsDeleted==false && x.IsDraft==false).Count();


            //sadece epostasına göre listeledik
            var query = _emailContext.Messages.Include(x => x.Category).AsQueryable();

            switch (folder.ToLower())
            {
                case "starred":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.IsStars == true && y.IsDraft == false && y.IsDeleted==false);
                    break;
                case "read":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.IsStatus == true && y.IsDraft==false && y.IsDeleted==false);
                    break;
                case "unread":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.IsStatus == false && y.IsDraft == false && y.IsDeleted == false);
                    break;
                case "draft":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.IsDraft == true && y.IsDeleted==false);
                    break;
                case "sent":
                    query = query.Where(x => x.SenderEmail==user.Email && x.IsDraft==false && x.IsDeleted==false);
                    break;
                case "deleted":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.IsDeleted == true);
                    break;
                case "job":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.CategoryId == 1 && y.IsDraft == false && y.IsDeleted == false);
                    break;
                case "education":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.CategoryId == 2 && y.IsDraft == false && y.IsDeleted == false);
                    break;
                case "family":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.CategoryId == 3 && y.IsDraft == false && y.IsDeleted == false);
                    break;
                case "social":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.CategoryId == 4 && y.IsDraft == false && y.IsDeleted == false);
                    break;
                case "other":
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.CategoryId == 5 && y.IsDraft == false && y.IsDeleted == false);
                    break;
                default:
                    query = query.Where(y => y.ReceiverEmail == user.Email && y.IsDraft == false && y.IsDeleted==false);
                    break;
            }

            var values = query.ToList();
            //var values = _emailContext.Messages.Include(x => x.Category).Where(y=>y.ReceiverEmail==user.Email && y.IsDraft==false && y.IsDeleted == false).ToList();
            return View(values);
        }
        public async Task< IActionResult> EmailDetail(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.image = user.ImageUrl;
            var values = _emailContext.Messages.Include(x=>x.Category).Where(x => x.MessageId == id).ToList();
            return View(values);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signinmanager.SignOutAsync();
            return RedirectToAction("UserLogin","Login");
        }
        public async Task< IActionResult> ProfileDetail()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.user = user.Name + " " + user.Surname;
            ViewBag.username = user.UserName;
            ViewBag.image = user.ImageUrl;
            ViewBag.mail = user.Email;
            if (user.EmailConfirmed==true)
            {
                ViewBag.confirmed = "Doğrulanmış Hesap";
            }
            else
            {
                ViewBag.confirmed = "Doğrulanmamış Hesap";
            }

            ViewBag.okunmuş = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStatus == true).Count();
            ViewBag.okunmamış = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStatus == false).Count();
            ViewBag.taslak = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsDraft == true).Count();
            ViewBag.yıldızlı = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStars == true).Count();
            ViewBag.çöp = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsDeleted == true).Count();
            ViewBag.mesaj = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email).Count();
            ViewBag.gönderilenmesaj = _emailContext.Messages.Where(x => x.SenderEmail == user.Email).Count();
            ViewBag.bugüngelenmesaj = _emailContext.Messages.Where(x => x.SenderEmail == user.Email && x.SendDate==DateTime.Today).Count();


            return View();
        }
        public IActionResult DeleteMail(int id)
        {
            var values = _emailContext.Messages.Find(id);
            values.IsDeleted = true;
            values.IsDraft = false;
            values.IsStars = false;
            _emailContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult StarsMail(int id)
        {
            var values = _emailContext.Messages.Find(id);
            values.IsStars = !values.IsStars;
            _emailContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ReadMail(int id)
        {
            var values = _emailContext.Messages.Find(id);
            values.IsStatus = !values.IsStatus;
            _emailContext.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult ReadMessage()
        {
            return View();
        }
        public IActionResult UnReadMessage()
        {
            return View();
        }
        public IActionResult StarsMessage()
        {
            return View();
        }
        public IActionResult DraftMessage()
        {
            return View();
        }
        public IActionResult SendMessage()
        {
            return View();
        }
        public IActionResult TrashMessage()
        {
            return View();
        }
        public IActionResult JobMessage()
        {
            return View();
        }
        public IActionResult FamilyMessage()
        {
            return View();
        }
        public IActionResult EducationMessage()
        {
            return View();
        }
        public IActionResult SocialMessage()
        {
            return View();
        }
        public IActionResult OtherMessage()
        {
            return View();
        }
    }
}
