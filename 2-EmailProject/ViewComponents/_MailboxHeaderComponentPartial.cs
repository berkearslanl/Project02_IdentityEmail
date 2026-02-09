using _2_EmailProject.Context;
using _2_EmailProject.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _2_EmailProject.ViewComponents
{
    public class _MailboxHeaderComponentPartial : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailContext _emailContext;

        public _MailboxHeaderComponentPartial(EmailContext emailContext, UserManager<AppUser> userManager)
        {
            _emailContext = emailContext;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var values = await _emailContext.Messages.Include(x => x.Category).Where(y => y.ReceiverEmail == user.Email && y.IsStatus==false && y.IsDeleted==false && y.IsDraft==false).ToListAsync();

            ViewBag.okunmamis = _emailContext.Messages.Where(x => x.ReceiverEmail == user.Email && x.IsStatus == false && x.IsDeleted==false && x.IsDraft==false).Count();


            return View(values);

            
        }
    }
}
