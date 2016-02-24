using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BGBC.Web.Controllers
{
    public class ReportsController : Controller
    {
        private IRepository<ContactForm, int> _contactForm;
        private IRepository<TenantReferral, int?> _tenantReferal;
        private IRepository<Tenant, int> _tenantRepo;
        private IRepository<BGBC.Model.Property, int> _pro;
        private IRepository<Product, int?> _product;
        IUserRepository _userRepository;
        IRepository<Property, int> _propertyRepo;

        public ReportsController()
        {
            _contactForm = new ContactRepository();
            _tenantReferal = new TenantRefRepository();
            _tenantRepo = new TenantRepository();
            _pro = new PropertyRepository();
            _product = new ProductRepository();
            _userRepository = new UserRepository();
            _propertyRepo = new PropertyRepository();

        }

        public ActionResult Contact()
        {
            return View(_contactForm.GetRef(1));
        }
        public ActionResult ContactRealtor()
        {
            return View(_contactForm.GetRef(1));
        }
        public ActionResult TenantReferal()
        {
            return View(_tenantReferal.Get());
        }
        public ActionResult PayoutPreferences()
        {
            return View(_userRepository.GetOwners());
        }
        public ActionResult AllProductsOrders(int? id)
        {
            List<vProductOrder> products = new List<vProductOrder>();
            try
            {
                if (id == null)
                {
                    products = BGBCFunctions.ProductOrders().ToList();
                }
                else
                {
                    products = BGBCFunctions.ProductOrders().Where(x => x.ProductID == id).ToList();
                }

            }
            catch
            {

            }
            return View(products);

        }
        public ActionResult PaymentHistory()
        {
            try
            {
                return View(BGBCFunctions.RentPayments());
            }
            catch
            {

            }
            return View(new List<vRentPayment>());
        }
        public ActionResult TenantPayments()
        {
            return View();
        }

        [Authorize]
        public ActionResult PropertyPaymentsHistory(int id)
        {
            List<vRentPayment> rentpay = new List<vRentPayment>();
            try
            {
                User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                Property property = _propertyRepo.Get((int)user.Properties.FirstOrDefault().PropertyID);
                ViewBag.Address = property.Address;
                ViewBag.Address2 = property.Address2;
                ViewBag.City = property.City;
                ViewBag.State = property.State;
                ViewBag.Zip = property.Zip;
                rentpay = BGBCFunctions.RentPayments().Where(x => x.PropertyID == id).ToList();
            }
            catch (Exception ex)
            {

            }
            return View(rentpay);
        }
        public ActionResult TenantPaymentHistory(int? id)
        {
            List<BGBC.Web.Models.TenantViewPayment> payment = new List<BGBC.Web.Models.TenantViewPayment>();
            try
            {
                List<vRentPayment> paymentdetails = BGBCFunctions.RentPayments()
                    .Where(x => x.TenantUserID == (id == null ? ((BGBC.Core.CustomPrincipal)(User)).UserId : id))
                    .OrderByDescending(d => d.TransDate).ToList();

                List<Tenant> tenants = _tenantRepo.Get().Where(x => x.UserID == (id == null ? ((BGBC.Core.CustomPrincipal)(User)).UserId : id)).ToList();

                foreach (Tenant tenant in tenants)
                {
                    BGBC.Web.Models.TenantViewPayment pay = new Models.TenantViewPayment();
                    pay.PropertyName = tenant.Property.Name;
                    pay.RentPayment = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == (id == null ? ((BGBC.Core.CustomPrincipal)(User)).UserId : id) && x.PropertyID == tenant.PropertyID).OrderByDescending(d => d.TransDate).ToList();
                    payment.Add(pay);
                }
            }
            catch
            {

            }
            return View(payment);
        }
        public ActionResult AllPropertiesAndTenant()
        {

            List<BGBC.Web.Models.AllPropertiesAndTenant> allPropertiesAndTenant = new List<Models.AllPropertiesAndTenant>();
            List<Property> property = _pro.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId);
            foreach (var p in property)
            {
                BGBC.Web.Models.AllPropertiesAndTenant pp = new Models.AllPropertiesAndTenant();
                pp.Pname = p.Name;
                pp.tenantRentPay = new List<Models.TenantRentPay>();
                foreach (var t in p.Tenants)
                {
                    BGBC.Web.Models.TenantRentPay ttttttttt = new BGBC.Web.Models.TenantRentPay();
                    ttttttttt.tname = t.User.FirstName;
                    ttttttttt.RentPayment = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == t.User.UserID).OrderByDescending(d => d.TransDate).Take(5).ToList();
                    pp.tenantRentPay.Add(ttttttttt);
                }
                allPropertiesAndTenant.Add(pp);
            }

            return View(allPropertiesAndTenant);
        }

        [Authorize]
        public ActionResult OrderHistory(int? id)
        {
            List<vProductOrder> payment = new List<vProductOrder>();
            try
            {
                payment = BGBCFunctions.ProductOrders().Where(x => x.UserID == ((BGBC.Core.CustomPrincipal)(User)).UserId).OrderByDescending(o => o.TransDate).ToList();
            }
            catch
            {

            }
            return View(payment);
        }

    }
}