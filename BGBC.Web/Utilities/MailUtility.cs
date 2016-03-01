using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

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
                Configuration configurationFile = WebConfigurationManager.OpenWebConfiguration("~/web.config");
                MailSettingsSectionGroup mailSettings = configurationFile
                    .GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
                if (mailSettings != null)
                {
                    int port = mailSettings.Smtp.Network.Port;
                    string host = mailSettings.Smtp.Network.Host;
                    string password = mailSettings.Smtp.Network.Password;
                    string username = mailSettings.Smtp.Network.UserName;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.To.Add(_toAddress);
                        mailMessage.Subject = _subject;
                        mailMessage.Body = _body;
                        mailMessage.IsBodyHtml = true;
                        var smtp = new SmtpClient();
                        smtp.DeliveryMethod = mailSettings.Smtp.DeliveryMethod;
                        smtp.Host = mailSettings.Smtp.Network.Host;
                        smtp.Credentials = new NetworkCredential(mailSettings.Smtp.Network.UserName, mailSettings.Smtp.Network.Password);
                        smtp.EnableSsl = mailSettings.Smtp.Network.EnableSsl;
                        smtp.Port = mailSettings.Smtp.Network.Port;
                        smtp.Send(mailMessage);
                        log.Info("Mail Sent to" + _toAddress);
                        return true;
                    }
                }
                else
                {
                    return false;
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
            this._toAddress = System.Configuration.ConfigurationManager.AppSettings.Get("AdminMailID").ToString();
            System.Text.StringBuilder _body = new System.Text.StringBuilder("<html><head><style>p { font-size: 12px; font-family: Arial;}</style></head><body>");
            _body.Append("<p>Hi BGBC Admin,</p>");
            _body.Append("<p>Tenant referral sent with following details.</p>");
            _body.Append("<p>Name: " + tenantRef.Name + "</p>");
            _body.Append("<p>Email: " + tenantRef.Email + "</p>");
            _body.Append("<p>Phone: " + tenantRef.Phone + "</p>");
            _body.Append("<p>Contact Name For Landlord/Property Management Company: " + tenantRef.LandlordName + "</p>");
            _body.Append("<p>Phone For Landlord/Property Management Company: " + tenantRef.LandlordPhone + "</p>");
            _body.Append("<p>Email For Landlord/Property Management Company: " + tenantRef.LandlordEmail + "</p>");
            _body.Append("<p>Mailing Address For Landlord/Property Management Company: " + tenantRef.LandlordAddress + "</p>");
            _body.Append("<br/><p>Thank you,<br/>BGBC Customer Service.</p>");
            _body.Append("</body></html>");

            try
            {
                IRepository<Email, int?> mailRepo = new EmailRepository();
                mailRepo.Add(new Email { ToAddress = this._toAddress, Subject = "Tenant Referral", Body = _body.ToString() });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        public void TenantPropertyManager(string SendMail)
        {

            var body = "<html><head><title></title></head><style>{ font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi,</span><br/><br/></td></tr><tr><td>" + SendMail + "<br/><br/></td></tr><tr><td></td></tr></br></br></br><tr><td>Thank you,</td></tr><tr><td>BGBC Customer Service.</td></tr></table></body></html>";

            _body = body;
            _subject = "Property Manager";
            this.Send();
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