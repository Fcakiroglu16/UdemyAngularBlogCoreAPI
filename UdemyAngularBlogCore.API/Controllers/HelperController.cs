using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mail;
using UdemyAngularBlogCore.API.Models;

namespace UdemyAngularBlogCore.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        [HttpPost]
        public IActionResult SendContactEmail(Contact contact)
        {
            System.Threading.Thread.Sleep(5000);
            try
            {
                MailMessage mailMessage = new MailMessage();

                SmtpClient smtpClient = new SmtpClient("mail.teknohub.net");

                mailMessage.From = new MailAddress("fcakiroglu@teknohub.net");
                mailMessage.To.Add("f-cakiroglu@outlook.com");

                mailMessage.Subject = contact.Subject;
                mailMessage.Body = contact.Message;
                mailMessage.IsBodyHtml = true;
                smtpClient.Port = 587;

                smtpClient.Credentials = new System.Net.NetworkCredential("fcakiroglu@teknohub.net", "FatihFatih31");

                smtpClient.Send(mailMessage);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}