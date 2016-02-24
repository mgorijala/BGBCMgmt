using BGBC.Core;
using BGBC.Model;
using BGBC.Model.Metadata;
using BGBC.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace BGBC.Web.Controllers
{
    public class OwnerController : Controller
    {
        private IRepository<Property, int> _propertyRepo;
        private IRepository<Tenant, int> _tenantRepo;
        private IRepository<BGBC.Model.Property, int> _pro;
        IUserRepository _userRepository;
        IRepository<Profile, int> _profileRepo;

        public OwnerController()
        {
            _propertyRepo = new PropertyRepository();
            _tenantRepo = new TenantRepository();
            _userRepository = new UserRepository();
            _profileRepo = new ProfileRepository();
            _pro = new PropertyRepository();
        }

        // GET: Owner
        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        public ActionResult Index()
        {
            PropertyDropDown();
            return View();
        }

        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        public ActionResult ViewPayments()
        {
            List<BGBC.Web.Models.AllPropertiesAndTenant> allPropertiesAndTenant = new List<Models.AllPropertiesAndTenant>();
            try
            {
                List<Property> property = _pro.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId);
                foreach (var p in property)
                {
                    BGBC.Web.Models.AllPropertiesAndTenant pp = new Models.AllPropertiesAndTenant();
                    pp.Pname = p.Name;
                    pp.Address = p.Address;
                    pp.Address2 = p.Address2;
                    pp.state = p.State;
                    pp.Zip = p.Zip;
                    pp.tenantRentPay = new List<Models.TenantRentPay>();
                    foreach (var t in p.Tenants)
                    {
                        BGBC.Web.Models.TenantRentPay ttttttttt = new BGBC.Web.Models.TenantRentPay();
                        ttttttttt.tname = t.User.FirstName;
                        ttttttttt.RentPayment = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == t.User.UserID).ToList();
                        pp.tenantRentPay.Add(ttttttttt);
                    }
                    allPropertiesAndTenant.Add(pp);
                }
            }
            catch (Exception ex)
            {

            }
            return View(allPropertiesAndTenant);
        }

        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        public ActionResult TenantViewPayments(int id)
        {
            //List<BGBC.Web.Models.TenantViewPayment> tenantViewPayment = new List<Models.TenantViewPayment>();
            List<vRentPayment> rentpay = new List<vRentPayment>();
            try
            {
                User tuser = _userRepository.Get(id);

                Profile profile = tuser.Profiles.FirstOrDefault();
                ViewBag.Name = tuser.FirstName + " " + tuser.LastName;
                ViewBag.Address = profile.BillingAddress;
                ViewBag.State = profile.BillingState;
                ViewBag.City = profile.BillingCty;
                ViewBag.Zip = profile.BillingZip;

                rentpay = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == id).ToList();
                return View(rentpay);

            }
            catch (Exception ex)
            {

            }
            return View(new List<vRentPayment>());
        }


        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        public ActionResult MyProperties()
        {
            return View(_propertyRepo.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId));
        }

        [CustomAuthorize(Roles = "Owner")]
        [Authorize]
        public ActionResult PropertyTenants(int id)
        {
            //Mohan code change is required to get property by owner id
            Property pro = _propertyRepo.Get(id);
            ViewBag.PropertyID = id;
            ViewBag.PropertyName = pro.Name;
            return View(BGBCFunctions.TenantOutstanding(id));
        }

        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        public ActionResult Add()
        {
            //Mohan code change is required to get property by owner id
            return View(_tenantRepo.Get());
        }

        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        public ActionResult Profile()
        {

            BGBC.Model.User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            Profile profile = _profileRepo.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            UserProfile userProfile = new TenantProfile { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, ConfirmEmail = user.Email, Createdon = user.Createdon, Updatedon = user.Updatedon, AltEmail = profile.AltEmail, ConfirmAltEmail = profile.AltEmail, ProfileInfo = profile };
            PopulateDropDown();
            return View(userProfile);
        }

        [HttpPost]
        [Authorize]
        [CustomAuthorize(Roles = "Owner")]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(UserProfile userprofile)
        {
            try
            {
                //Adding Regularexpression 
                
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.ProfileInfo.BillingAddress, false, "Address", "ProfileInfo.BillingAddress");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.ProfileInfo.BillingAddress_2, false, "Address 2", "ProfileInfo.BillingAddress_2");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.ProfileInfo.BillingCty, false, "City", "ProfileInfo.BillingCty");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.ProfileInfo.BillingState, false, "State", "ProfileInfo.BillingState");
                BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, userprofile.ProfileInfo.BillingZip, false, "Zip", "ProfileInfo.BillingZip");

                if (userprofile.ProfileInfo.PaymentMethod == "PayPal Email")
                {
                    if (string.IsNullOrEmpty(userprofile.ProfileInfo.PaypalEmail)) ModelState.AddModelError("ProfileInfo.PaypalEmail", "The Paypal Email field is required");
                    BGBC.Core.ModelDataValidation.Instance.Email(ModelState, userprofile.ProfileInfo.PaypalEmail, true, "PayPal Email", "ProfileInfo.PaypalEmail");
                }

                if (userprofile.ProfileInfo.PaymentMethod == "Mail Check")
                {
                    if (string.IsNullOrEmpty(userprofile.ProfileInfo.PayoutMailAddress)) ModelState.AddModelError("ProfileInfo.PayoutMailAddress", "The Mailling address field is required");
                    if (string.IsNullOrEmpty(userprofile.ProfileInfo.PayoutMailAddress2)) ModelState.AddModelError("ProfileInfo.PayoutMailAddress2", "The Mailling address 2 field is required");
                    if (string.IsNullOrEmpty(userprofile.ProfileInfo.PayoutMailCity)) ModelState.AddModelError("ProfileInfo.PayoutMailCity", "The Mailling City field is required");
                    if (string.IsNullOrEmpty(userprofile.ProfileInfo.PayoutMailState)) ModelState.AddModelError("ProfileInfo.PayoutMailState", "The Mailling State field is required");
                    if (string.IsNullOrEmpty(userprofile.ProfileInfo.PayoutMailZip)) ModelState.AddModelError("ProfileInfo.PayoutMailZip", "The Mailling Zip field is required");


                    BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.ProfileInfo.PayoutMailAddress, false, "Mailing Address", "ProfileInfo.PayoutMailAddress");
                    BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.ProfileInfo.PayoutMailAddress2, false, "Mailing Address 2", "ProfileInfo.PayoutMailAddress2");
                    BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.ProfileInfo.PayoutMailCity, false, "Mailing City", "ProfileInfo.PayoutMailCity");
                    BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.ProfileInfo.PayoutMailState, false, "Mailing State", "ProfileInfo.PayoutMailState");
                    BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, userprofile.ProfileInfo.PayoutMailZip, false, "Mailing Zip", "ProfileInfo.PayoutMailZip");
                }

                if (ModelState.IsValid)
                {
                    User selUser = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                    if (!string.IsNullOrEmpty(userprofile.NewPassword))
                    {
                        if (selUser.Password == BGBC.Core.Security.Cryptography.Encrypt(userprofile.CurrentPassword))
                        {
                            selUser.Password = BGBC.Core.Security.Cryptography.Encrypt(userprofile.NewPassword);
                        }
                        else
                        {
                            ModelState.AddModelError("CurrentPassword", "Invalid password");
                            return View(userprofile);
                        }
                    }
                    selUser.FirstName = userprofile.FirstName;
                    selUser.LastName = userprofile.LastName;
                    selUser.Email = userprofile.Email;
                    userprofile.ProfileInfo.UserID = selUser.UserID;
                    userprofile.ProfileInfo.AltEmail = userprofile.AltEmail;
                    _userRepository.Update(selUser);
                    _profileRepo.Update(userprofile.ProfileInfo);
                    TempData["SucessMessage"] = "Profile Updated Successfully";
                    return RedirectToAction("Profile", "Owner");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDropDown();
            return View(userprofile);


        }

        private void PopulateDropDown()
        {
            ViewBag.States = new List<SelectListItem>()
            {
                new SelectListItem() {Text="Alabama", Value="AL"},
                new SelectListItem() { Text="Alaska", Value="AK"},
                new SelectListItem() { Text="Arizona", Value="AZ"},
                new SelectListItem() { Text="Arkansas", Value="AR"},
                new SelectListItem() { Text="California", Value="CA"},
                new SelectListItem() { Text="Colorado", Value="CO"},
                new SelectListItem() { Text="Connecticut", Value="CT"},
                new SelectListItem() { Text="District of Columbia", Value="DC"},
                new SelectListItem() { Text="Delaware", Value="DE"},
                new SelectListItem() { Text="Florida", Value="FL"},
                new SelectListItem() { Text="Georgia", Value="GA"},
                new SelectListItem() { Text="Hawaii", Value="HI"},
                new SelectListItem() { Text="Idaho", Value="ID"},
                new SelectListItem() { Text="Illinois", Value="IL"},
                new SelectListItem() { Text="Indiana", Value="IN"},
                new SelectListItem() { Text="Iowa", Value="IA"},
                new SelectListItem() { Text="Kansas", Value="KS"},
                new SelectListItem() { Text="Kentucky", Value="KY"},
                new SelectListItem() { Text="Louisiana", Value="LA"},
                new SelectListItem() { Text="Maine", Value="ME"},
                new SelectListItem() { Text="Maryland", Value="MD"},
                new SelectListItem() { Text="Massachusetts", Value="MA"},
                new SelectListItem() { Text="Michigan", Value="MI"},
                new SelectListItem() { Text="Minnesota", Value="MN"},
                new SelectListItem() { Text="Mississippi", Value="MS"},
                new SelectListItem() { Text="Missouri", Value="MO"},
                new SelectListItem() { Text="Montana", Value="MT"},
                new SelectListItem() { Text="Nebraska", Value="NE"},
                new SelectListItem() { Text="Nevada", Value="NV"},
                new SelectListItem() { Text="New Hampshire", Value="NH"},
                new SelectListItem() { Text="New Jersey", Value="NJ"},
                new SelectListItem() { Text="New Mexico", Value="NM"},
                new SelectListItem() { Text="New York", Value="NY"},
                new SelectListItem() { Text="North Carolina", Value="NC"},
                new SelectListItem() { Text="North Dakota", Value="ND"},
                new SelectListItem() { Text="Ohio", Value="OH"},
                new SelectListItem() { Text="Oklahoma", Value="OK"},
                new SelectListItem() { Text="Oregon", Value="OR"},
                new SelectListItem() { Text="Pennsylvania", Value="PA"},
                new SelectListItem() { Text="Rhode Island", Value="RI"},
                new SelectListItem() { Text="South Carolina", Value="SC"},
                new SelectListItem() { Text="South Dakota", Value="SD"},
                new SelectListItem() { Text="Tennessee", Value="TN"},
                new SelectListItem() { Text="Texas", Value="TX"},
                new SelectListItem() { Text="Utah", Value="UT"},
                new SelectListItem() { Text="Vermont", Value="VT"},
                new SelectListItem() { Text="Virginia", Value="VA"},
                new SelectListItem() { Text="Washington", Value="WA"},
                new SelectListItem() { Text="West Virginia", Value="WV"},
                new SelectListItem() { Text="Wisconsin", Value="WI"},
                new SelectListItem() { Text="Wyoming", Value="WY"}
            };
        }



        private void PropertyDropDown()
        {
            List<SelectListItem> property = new List<SelectListItem>();
            List<SelectListItem> tenants = new List<SelectListItem>();
            foreach (var item in _propertyRepo.Get())
            {
                property.Add(new SelectListItem() { Text = item.Name, Value = Url.Action("PropertyPaymentsHistory", "Reports", new { id = item.PropertyID }) });
            }
            foreach (var item in _tenantRepo.Get())
            {
                tenants.Add(new SelectListItem() { Text = (item.User).FirstName, Value = Url.Action("TenantPaymentHistory", "Reports", new { id = item.UserID }) });

            }
            ViewBag.AllProperties = property;
            ViewBag.AllTenants = tenants;
        }
    }
}