using BGBC.Core;
using BGBC.Model;
using BGBC.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BGBC.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        IUserRepository _userRepository;
        IRepository<Profile, int> _profileRepo;
        private IRepository<ContactForm, int> _repository;
        private IRepository<TenantReferral, int?> _tenantRefRepo;
        IRepository<PasswordReset, int?> passwordresetRepo;
        UserRepository repos = new UserRepository();

        public HomeController()
        {
            _userRepository = new UserRepository();
            _profileRepo = new ProfileRepository();
            _repository = new ContactRepository();
            _tenantRefRepo = new TenantRefRepository();
            passwordresetRepo = new PasswordResetRepository();
        }
        public ActionResult Index()
        {
            return View();
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Contact()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Contact(BGBC.Web.Models.Contact contact)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContactForm contactform = new ContactForm();
                    contactform.MessageType = 1;
                    contactform.AccountName = contact.AccountName;
                    contactform.Name = contact.Name;
                    contactform.EMail = contact.EMail;
                    contactform.Phone = contact.Phone;
                    contactform.MessageText = contact.MessageText;
                    _repository.Add(contactform);

                    MailSending mailsend = new MailSending();
                    //mailsend.ContactUs("webmail@appstekcorp.com", contactform.Name, contactform.EMail, contactform.MessageText, contactform.Phone);

                    TempData["SucessMessage"] = "Message Successfully Send.";
                    return RedirectToAction("Contact", "Home");

                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(contact);
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(User user)
        {
            try
            {
                User selUser = repos.Find(user.Email);
                if (selUser != null)
                {
                    var token = BGBC.Core.Security.Cryptography.RandomString(32);
                    PasswordReset passwordreset = new PasswordReset();
                    passwordreset.EmailID = user.Email;
                    passwordreset.Token = token;
                    passwordresetRepo.Add(passwordreset);
                    BGBC.Web.Utilities.MailUtility obj = new Utilities.MailUtility();
                    obj.ForgetPassword(selUser.Email, selUser.FirstName + " " + selUser.LastName, "http://" + HttpContext.Request.Url.Authority + "/Home/ResetPassword/" + token);
                    TempData["SucessMessage"] = "Message Successfully Send.";
                }
                else
                    TempData["SucessMessage"] = "Mail id not exists.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string id)
        {
            BGBC.Web.Models.ResetPassword reset = new ResetPassword();
            try
            {
                PasswordReset pwd = passwordresetRepo.Get().Where(x => x.Token == id).FirstOrDefault();
                if (pwd == null)
                { ViewBag.ValidEmail = false; }
                else
                { reset.TokenID = id; ViewBag.ValidEmail = true; }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(reset);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(ResetPassword resetpassword)
        {
            try
            {
                PasswordReset pwd = passwordresetRepo.Get().Where(x => x.Token == resetpassword.TokenID).FirstOrDefault();
                if (pwd != null)
                {
                    User user = _userRepository.Find(pwd.EmailID);
                    user.Password = BGBC.Core.Security.Cryptography.Encrypt(resetpassword.Password);
                    _userRepository.Update(user);
                    TempData["SucessMessage"] = "Your Password Successfully Changed.";
                    passwordresetRepo.Remove(pwd);
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    TempData["SucessMessage"] = "Link has been expired. Please log new Forgot Password Request.";
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            ViewBag.ValidEmail = false;
            return View(resetpassword);
        }
        [HttpGet]
        public ActionResult ListHome()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListHome(BGBC.Web.Models.Contact findlisthome)
        {
            try
            {
                ModelState["AccountName"].Errors.Clear();
                ModelState["MessageText"].Errors.Clear();
                if (ModelState.IsValid)
                {

                    ContactForm contactform = new ContactForm();
                    contactform.Name = findlisthome.Name;
                    contactform.EMail = findlisthome.EMail;
                    contactform.Phone = findlisthome.Phone;
                    contactform.MessageText = findlisthome.MessageText;
                    contactform.MessageType = 2;
                    _repository.Add(contactform);

                    MailSending mailsend = new MailSending();
                    //mailsend.ContactRealtor("webmail@appstekcorp.com", contactform.Name, contactform.EMail, contactform.MessageText, contactform.Phone);

                    TempData["SucessMessage"] = "Successfully Submit.";
                    return RedirectToAction("ListHome", "Home");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(findlisthome);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(User user)
        {
            try
            {
                IUserRepository repo = new UserRepository();
                User selUser = repo.Find(user.Email, BGBC.Core.Security.Cryptography.Encrypt(user.Password));

                if (selUser != null)
                {
                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                    serializeModel.UserId = selUser.UserID;
                    serializeModel.FirstName = selUser.FirstName;
                    serializeModel.LastName = selUser.LastName;

                    if (selUser.UserType == 0)
                        serializeModel.roles = new string[] { "Admin" };
                    else if (selUser.UserType == 1)
                        serializeModel.roles = new string[] { "Owner" };
                    else if (selUser.UserType == 2)
                        serializeModel.roles = new string[] { "Tenant" };
                    else if (selUser.UserType == 3)
                        serializeModel.roles = new string[] { "Customer" };

                    string userData = JsonConvert.SerializeObject(serializeModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, selUser.FirstName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    if (selUser.UserType == 0)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (selUser.UserType == 1)
                    {
                        return RedirectToAction("Index", "Owner");
                    }
                    else if (selUser.UserType == 2)
                    {
                        return RedirectToAction("Index", "Tenant");
                    }
                    else if (selUser.UserType == 3)
                    {
                        return RedirectToAction("Index", "Customer");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The username or password you provided is invalid.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(user);
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home", null);
        }

        [AllowAnonymous]
        public ActionResult PaymentProcess()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult RentalManagement()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Tenants()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(Register register)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    User user = _userRepository.Find(register.Email);
                    if (user != null)
                    {
                        ModelState.AddModelError("Email", "Email is already exists");
                        return View(register);
                    }
                    else
                    {
                        user = new Model.User();
                        user.FirstName = register.FirstName;
                        user.LastName = register.LastName;

                        if (register.UserType == 1)
                        {
                            user.UserType = 1;
                        }
                        else
                        {
                            user.UserType = 3;
                        }

                        user.Email = register.Email;
                        user.Password = BGBC.Core.Security.Cryptography.Encrypt(register.Password);
                        user.Profiles.Add(new Profile());
                        _userRepository.Add(user);

                        MailSending mailsend = new MailSending();
                        //mailsend.UserRegister(register.Email, register.FirstName, register.Email, register.Password);

                        CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                        serializeModel.UserId = user.UserID;
                        serializeModel.FirstName = user.FirstName;
                        serializeModel.LastName = user.LastName;
                        if (user.UserType == 1)
                            serializeModel.roles = new string[] { "Owner" };
                        else if (user.UserType == 3)
                            serializeModel.roles = new string[] { "Customer" };


                        string userData = Newtonsoft.Json.JsonConvert.SerializeObject(serializeModel);
                        System.Web.Security.FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, user.FirstName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);

                        string encTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                        Response.Cookies.Add(faCookie);

                        if (user.UserType == 1)
                        {
                            return RedirectToAction("Index", "Owner");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Customer");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(register);
        }

        [AllowAnonymous]
        public ActionResult TenantReferral()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult TenantReferral(TenantReferral tenantReferral)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    _tenantRefRepo.Add(tenantReferral);
                    MailSending mailsend = new MailSending();
                    //mailsend.PropertyManagement(tenantReferral.Email);
                    TempData["SucessMessage"] = "Tenant referred successfully";
                    return View();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View(tenantReferral);
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

    }

}