using _2_EmailProject.Context;
using _2_EmailProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2_EmailProject.Controllers
{
    public class MessageController : Controller
    {
        private readonly EmailContext _context;
        private readonly UserManager<AppUser> _userManager;

        public MessageController(EmailContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateMessage()
        {
            ViewBag.categories = (from x in _context.Categories.ToList()
                                  select new SelectListItem
                                  {
                                      Text = x.CategoryName,
                                      Value = x.CategoryId.ToString()
                                  }).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(Message message,string actionType)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            message.SenderEmail = user.Email;

            message.SendDate = DateTime.Now;
            message.IsStatus = false;

            if (actionType=="draft")
            {
                message.IsDraft = true;
                _context.Messages.Add(message);
                _context.SaveChanges();
                return RedirectToAction("Index","Mailbox");
            }
            else
            {
                message.IsDraft= false;
                _context.Messages.Add(message);
                _context.SaveChanges();
                return RedirectToAction("Index", "Mailbox");
            }

                
        }
        public async Task<IActionResult> Inbox()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            ViewBag.mail = user.Email;
            ViewBag.okunmamismesaj = _context.Messages.Where(x => x.IsStatus == false).Count();
            var messageList = _context.Messages.Where(x => x.ReceiverEmail == user.Email).ToList();
            return View(messageList);
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                message.IsStatus = true;
                await _context.SaveChangesAsync();
                return Ok();
            }
            
            return NotFound();
        }
        public IActionResult MessageDelete(int id)
        {
            var values = _context.Messages.Find(id);
            if (values != null)
            {
                _context.Messages.Remove(values);
                _context.SaveChanges();
                return RedirectToAction("Inbox");
            }
            return View();
        }
    }
}
