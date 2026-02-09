using Microsoft.AspNetCore.Mvc;

namespace _2_EmailProject.ViewComponents
{
    public class _MailboxMainComponentPartial:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
