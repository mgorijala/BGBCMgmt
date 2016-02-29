using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace BGBC.Web.Utilities
{
    public class MailUtility
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(MailUtility));
        private const int Timeout = 180000;
        private string _toAddress;
        private string _subject;
        private string _body;

        private bool Send()
        {
            try
            {
                using (var mailMessage = new MailMessage())
                {
                    mailMessage.To.Add(_toAddress);
                    mailMessage.Subject = _subject;
                    mailMessage.Body = _body;
                    mailMessage.IsBodyHtml = true;
                    var smtp = new SmtpClient();
                    smtp.Credentials = new NetworkCredential();
                    smtp.EnableSsl = false;
                    smtp.Send(mailMessage);
                    return true;
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                log.Error(ex.Message);
                return false;
            }
        }

        public bool Send(string ToAddress, string Subject, string Body)
        {
            this._toAddress = ToAddress;
            this._subject = Subject;
            this._body = Body;
            return this.Send();
        }

        public void ForgetPassword(string ToAddress, string Name, string ResetUrl)
        {
            this._toAddress = ToAddress;
            this._subject = "Forgot password";
            _body = "<html><head><title></title></head><style>td { font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi " + Name + ",</span><br/><br/></td></tr><tr><td>You requested a password reset. Please visit this link to enter your new password.</td></tr><tr><td></td></tr><tr><td><a href='" + ResetUrl + "'>" 
                + ResetUrl + "</a><br/><br/><br/></td></tr></br><tr><td>Thank you,</td></tr><tr><td>BGBC Customer Service.</td></tr></table></body></html>";
            this.Send();
        }

        public void TenantAdd(string ToAddress, string Name, string ResetUrl)
        {
            this._toAddress = ToAddress;
            this._subject = "Tenant User Created";
            _body = "<html><head><title></title></head><style>td { font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi " + Name + ",</span><br/><br/></td></tr><tr><td>New login has been created by your property owner. Please visit this link to enter your new password.</td></tr><tr><td></td></tr><tr><td><a href='" + ResetUrl + "'>"
                + ResetUrl + "</a><br/><br/><br/></td></tr></br><tr><td>Thank you,</td></tr><tr><td>BGBC Customer Service.</td></tr></table></body></html>";
            IRepository<Email, int?> mailRepo = new EmailRepository();
            mailRepo.Add(new Email { ToAddress = ToAddress, Subject = _subject, Body = _body });
        }

        public void TenantReferral(BGBC.Model.TenantReferral tenantRef)
        {

            var body = "<html><head><title></title></head><style>{ font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi,</span><br/><br/></td></tr><tr><td><br/><br/><br/></td></tr></table></body></html>";

            _body = body;
            _subject = "Tenant Referral";

            try
            {
                IRepository<Email, int?> mailRepo = new EmailRepository();
                mailRepo.Add(new Email { ToAddress = System.Configuration.ConfigurationManager.AppSettings.Get("Key1").ToString(), Subject = _subject, Body = _body });
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class EmailJob : Quartz.IJob
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(EmailJob));
        public void Execute(Quartz.IJobExecutionContext context)
        {
            List<Email> emailjob = new List<Email>();
            IRepository<Email, int?> mailRepo = new EmailRepository();
            try
            {
                MailUtility mailUtility = new MailUtility();
                foreach (Email msg in mailRepo.Get().Where(x => x.Active == true).ToList())
                {
                    if (mailUtility.Send(msg.ToAddress, msg.Subject, msg.Body))
                    {
                        log.Info("Mail Sent: " + msg.ToAddress + " " + msg.Subject);
                        mailRepo.Remove(msg);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}