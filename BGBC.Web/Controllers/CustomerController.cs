using BGBC.Core;
using BGBC.Model;
using BGBC.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BGBC.Web.Controllers
{
    public class CustomerController : Controller
    {
        IUserRepository _userRepository;
        private IRepository<Property, int> _propertyRepo;
        private IRepository<Tenant, int> _tenantRepo;
        private IRepository<Product, int?> _producRepo;
        private IRepository<Profile, int> _profileRepo;

         public CustomerController()
        {
            _propertyRepo = new PropertyRepository();
            _userRepository = new UserRepository();
            _tenantRepo = new TenantRepository();
            _producRepo = new ProductRepository();
            _profileRepo = new ProfileRepository();
        }

        // GET: Customer
        [CustomAuthorize(Roles = "Customer")]
        [Authorize]
        public ActionResult Index()
        {
           BGBC.Model.User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);

           return View(user);
        }
        [Authorize]
        public ActionResult Profile()
        {
            PopulateDropDown();
            BGBC.Model.User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            Profile profile = _profileRepo.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            UserProfile userProfile = new UserProfile { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, ConfirmEmail = user.Email, Createdon = user.Createdon, Updatedon = user.Updatedon, AltEmail = profile.AltEmail, ProfileInfo = profile};
            return View(userProfile);
           
        }

        [HttpPost, ActionName("Profile")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ProfilePost(UserProfile userprofile)
        {
            try
            {
                if (string.IsNullOrEmpty(userprofile.ProfileInfo.BillingAddress)) ModelState.AddModelError("ProfileInfo.BillingAddress", "The Address field is required.");
                if (string.IsNullOrEmpty(userprofile.ProfileInfo.BillingCty)) ModelState.AddModelError("ProfileInfo.BillingCty", "The City field is required.");
                if (string.IsNullOrEmpty(userprofile.ProfileInfo.BillingState)) ModelState.AddModelError("ProfileInfo.BillingState", "The State field is required.");
                if (string.IsNullOrEmpty(userprofile.ProfileInfo.BillingZip)) ModelState.AddModelError("ProfileInfo.BillingZip", "The Zip field is required.");

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
                    }
                }
                    selUser.FirstName = userprofile.FirstName;
                    selUser.LastName = userprofile.LastName;
                    selUser.Email = userprofile.Email;
                    _userRepository.Update(selUser);

                    Profile _profile = _profileRepo.Get(selUser.Profiles.FirstOrDefault().ProfileID);
                    _profile.AltEmail = userprofile.AltEmail;
                    _profile.BillingAddress = userprofile.ProfileInfo.BillingAddress;
                    _profile.BillingAddress_2 = userprofile.ProfileInfo.BillingAddress_2;
                    _profile.BillingCty = userprofile.ProfileInfo.BillingCty;
                    _profile.BillingState = userprofile.ProfileInfo.BillingState;
                    _profile.BillingZip = userprofile.ProfileInfo.BillingZip;
                        
         
                    _profileRepo.Update(_profile);


                    TempData["SucessMessage"] = "Customer profile updated successfully.";
                    return RedirectToAction("Profile", "Customer");
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
    }
}