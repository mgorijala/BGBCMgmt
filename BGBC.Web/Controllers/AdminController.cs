using BGBC.Core;
using BGBC.Model;
using BGBC.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace BGBC.Web.Controllers
{
    public class AdminController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AdminController));
        IUserRepository _userRepository;
        private IRepository<Property, int> _propertyRepo;
        private IRepository<Tenant, int> _tenantRepo;
        private IRepository<Product, int?> _producRepo;
        //private IRepository<Payment, int> _payment;

        public AdminController()
        {
            _propertyRepo = new PropertyRepository();
            _userRepository = new UserRepository();
            _tenantRepo = new TenantRepository();
            _producRepo = new ProductRepository();
            //_payment = new PaymentRepository();
        }


        // GET: Dashboard
        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            propertynewDropDown();
            return View();
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ManageProperty()
        {
            return View(_userRepository.GetOwners());
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult OwnerProperties(int id)
        {
            return View(_propertyRepo.Get().Where(x => x.UserID == id));
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Profile()
        {

            BGBC.Model.User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            UserProfile userProfile = new TenantProfile { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, ConfirmEmail = user.Email, Createdon = user.Createdon, Updatedon = user.Updatedon };
            return View(userProfile);
        }

        [HttpPost, ActionName("Profile")]
        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult ProfilePost(UserProfile userprofile)
        {
            try
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
                if (ModelState.IsValid)
                {
                    selUser.FirstName = userprofile.FirstName;
                    selUser.LastName = userprofile.LastName;
                    selUser.Email = userprofile.Email;
                    _userRepository.Update(selUser);
                    TempData["SucessMessage"] = "Admin profile updated successfully.";
                    return RedirectToAction("Profile", "Admin");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(userprofile);


        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PropertyTenants(int id)
        {
                
            Property pro = _propertyRepo.Get(id);
            ViewBag.PropertyID = id;
            ViewBag.PropertyName = pro.Name;
            ViewBag.UserId = pro.UserID;

            //User userid = _propertyRepo.Get().Where(x => x.UserID == id);
            //ViewBag.UserId = userid;

            return View(BGBCFunctions.TenantOutstanding(id));
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult AllPropertyTenants(int id)
        {

            return View(_propertyRepo.GetRef(id));
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ViewTenantsPayment(int id)
        {

            List<vRentPayment> rentpay = new List<vRentPayment>();
            try
            {

                User tuser = _userRepository.Get(id);

                Profile profile = tuser.Profiles.FirstOrDefault();
                ViewBag.Name = tuser.FirstName + " " + tuser.LastName;
                rentpay = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == id).ToList();
                return View(rentpay);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(new List<vRentPayment>());
        }

        private void propertynewDropDown()
        {
            List<SelectListItem> produts = new List<SelectListItem>();
            foreach (var item in _producRepo.Get())
            {
                produts.Add(new SelectListItem() { Text = item.Name, Value = Url.Action("AllProductsOrders", "Reports", new { id = item.ProductID }) });
            }

            ViewBag.AllProducts = produts;
        }
    }
}