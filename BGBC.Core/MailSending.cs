using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace BGBC.Core
{
   
    public class MailSending
    {
        private const int Timeout = 180000;
        private string _sender;
        private string _recipient;
        private string _recipientCC;
        private string _subject;
        private string _body;

        public MailSending()
        {
            _sender = "webmail@appstekcorp.com";
        }

        private void Send()
        {
            try
            {
                var message = new MailMessage(_sender, _recipient,_subject, _body) { IsBodyHtml = true };
                var smtp = new SmtpClient();
                smtp.Credentials = new NetworkCredential();
                smtp.Send(message);
                message.Dispose();
                smtp.Dispose();
            }
            catch (Exception ex)
            {

            }
        }      
        public void UserRegister(string toadd,string name,string email,string password)
        {
            var body="<html><head><title></title></head><style>td { font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi &nbsp;" + name + ",</span><br/><br/></td></tr><tr><td>You email is:<b>" + email + "</b></td></tr><tr><td>your password is:<b>" + password + " </b></font></td></tr><tr><td></font></td></tr><tr><td></td></tr></table></body></html>";
            _recipient = toadd;
            _body = body;
            _subject = "BGBC";
            this.Send();
        }

        public void ForgetPassword(string toadd)
        {            
            _recipient = toadd;
            _body = "<a href='http://localhost:43580/Home/ResetPassword'>Click this link to change your password</a>";
            _subject = "BGBC";
            this.Send();
        }

        public void ContactRealtor(string toadd, string name, string email, string subject, string number)
        {
            var body = "<html><head><title></title></head><style>td { font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi,</span><br/><br/></td></tr><tr><td>" + subject + "<br/><br/><br/></td></tr><tr><td><b>" + name + " </font></b></td></tr><tr><td>" + email + "</font></td></tr><tr><td>" + number + "</td></tr></table></body></html>";
            _recipient = toadd;
            _body = body;
            _subject = "BGBC";
            this.Send();
        }
        public void ContactUs(string toadd,string name,string email,string subject,string number)
        {
            var body = "<html><head><title></title></head><style>td { font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi,</span><br/><br/></td></tr><tr><td>" + subject + "<br/><br/><br/></td></tr><tr><td><b>" + name + " </font></b></td></tr><tr><td>" + email + "</font></td></tr><tr><td>" + number + "</td></tr></table></body></html>";
            _recipient = toadd;
            _recipientCC = toadd;
            _body = body;
            _subject = "Contact form";
            this.Send();
        }
        public void PropertyManagement(string toadd)
        {
            var body="<html><head><title></title></head><style>td { font-size: 12px; font-family: Arial;}</style><body><table><tr><td><span>Hi,</span><br/><br/></td></tr><tr><td><br/><br/><br/></td></tr></table></body></html>";
            _recipient=toadd;
            _body=body;
            _subject="BGBC";
            this.Send();
        }
        public void OrderConfirmation(string toadd)
        {
            _recipient = toadd;
            _body = "Yor order is success";
            _subject = "BGBC";
            this.Send();
        }
        public void ConfirmPayment(string toadd)
        {
            _recipient = toadd;
            _body = "Yor payment is ";
            _subject = "BGBC";
            this.Send();
        }


    }

}
