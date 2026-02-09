using _2_EmailProject.Dtos;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace _2_EmailProject.Controllers
{
    public class EmailController : Controller
    {
        [HttpGet]
        public IActionResult SendEmail()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SendEmail(MailRequestDto mailRequestDto)
        {
            MimeMessage mimeMessage = new MimeMessage();

            MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "berkesude39@gmail.com");
            mimeMessage.From.Add(mailboxAddressFrom);

            MailboxAddress mailboxAddressTo = new MailboxAddress("User", mailRequestDto.ReceiverEmail);
            mimeMessage.To.Add(mailboxAddressTo);

            mimeMessage.Subject = mailRequestDto.Subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = mailRequestDto.MessageDetail;
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 587, false);
            smtpClient.Authenticate("berkesude39@gmail.com", "ttym seem mhda yxxp");
            smtpClient.Send(mimeMessage);
            smtpClient.Disconnect(true);

            return RedirectToAction("Inbox", "Message");
        }

    }
}
