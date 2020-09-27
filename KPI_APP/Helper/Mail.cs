using KPI_APP.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KPI_APP.Helper
{
    public static class Mail
    {
        public static async Task SendEmail(
            string Subject, string Body,
            string From, string To, string CC,
            Attachment Attachment = null, byte EmailType = 0)
        {

            var SmtpPassword = ConfigurationManager.AppSettings["Password"];
            var SmtpEmail = ConfigurationManager.AppSettings["Email"];

            SmtpClient smtp = new SmtpClient("mail-emea.msc.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(SmtpEmail, SmtpPassword)
            };

            MailMessage msg = new MailMessage()
            {
                From = new MailAddress(From),
                Subject = Subject,
                Body = Body,
                IsBodyHtml = true
            };

            string[] ToMuliId = To.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ToEMailId in ToMuliId)
            {
                if (!string.IsNullOrEmpty(ToEMailId))
                    msg.To.Add(new MailAddress(ToEMailId));
            }

            if (Attachment != null) msg.Attachments.Add(Attachment);

            if (!string.IsNullOrEmpty(CC))
            {
                string[] CCId = CC.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string CCEmail in CCId)
                {
                    if (!string.IsNullOrEmpty(CCEmail))
                        msg.CC.Add(new MailAddress(CCEmail));
                }
            }

            try
            {
                smtp.Send(msg);
                using (var db = new Context())
                {
                    db.Emails.Add(new Emails() { EmailType = EmailType, CreateDate = DateTime.Now });
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(" \n\r ");
                sb.Append(ex.Message + "  " + DateTime.Now.ToString() + " \n\r ");
                System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/") + "log.txt", sb.ToString());
                sb.Clear();
            }



        }
    }
}