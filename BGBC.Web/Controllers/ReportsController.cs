using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace BGBC.Web.Controllers
{
    public class ReportsController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReportsController));
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
        public ActionResult AllProductsOrders(int? id, string sortOrder, string currentFilter, string searchString, int? page, string PageSize)
        {
            List<vProductOrder> products = new List<vProductOrder>();
            int pageSize = int.Parse(PageSize == null ? "3" : PageSize), pageNumber = (page ?? 1);
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

                ViewBag.CurrentSort = sortOrder;
                ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
                ViewBag.OrderSortParm = sortOrder == "OrderID" ? "OrderID_desc" : "OrderID";
                ViewBag.CustomerSortParm = sortOrder == "Customer" ? "Customer_desc" : "Customer";
                ViewBag.TypeSortParm = sortOrder == "Type" ? "Type_desc" : "Type";
                ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";

                ViewBag.PageSize = pageSize;


                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(x => x.CustomerFName.Contains(searchString) || x.CustomerLName.Contains(searchString)
                        || x.Name.Contains(searchString) || x.TransId.Contains(searchString)).ToList();

                }

                switch (sortOrder)
                {
                    case "date_desc":
                        products = products.OrderByDescending(x => x.TransDate).ToList();
                        break;
                    case "OrderID":
                        products = products.OrderBy(x => x.TransId).ToList();
                        break;
                    case "OrderID_desc":
                        products = products.OrderByDescending(x => x.TransId).ToList();
                        break;
                    case "Customer":
                        products = products.OrderBy(x => x.CustomerFName).ToList();
                        break;
                    case "Customer_desc":
                        products = products.OrderByDescending(x => x.CustomerFName).ToList();
                        break;
                    case "Type":
                        products = products.OrderBy(x => x.UserType).ToList();
                        break;
                    case "Type_desc":
                        products = products.OrderByDescending(x => x.UserType).ToList();
                        break;
                    case "Price":
                        products = products.OrderBy(x => x.Price).ToList();
                        break;
                    case "Price_desc":
                        products = products.OrderByDescending(x => x.Price).ToList();
                        break;
                    default:  // Date ascending 
                        products = products.OrderByDescending(x => x.TransDate).ToList();
                        break;
                }


            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(products.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult PaymentHistory()
        {
            try
            {
                return View(BGBCFunctions.RentPayments());
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
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
                log.Error(ex.Message);
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
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(payment);
        }
        public ActionResult AllPropertiesAndTenant()
        {

            List<BGBC.Web.Models.AllPropertiesAndTenant> allPropertiesAndTenant = new List<Models.AllPropertiesAndTenant>();
            List<Property> property = _pro.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId);
            foreach (var p in property)
            {
                BGBC.Web.Models.AllPropertiesAndTenant allProps = new Models.AllPropertiesAndTenant();
                allProps.Pname = p.Name;
                allProps.tenantRentPay = new List<Models.TenantRentPay>();
                foreach (var t in p.Tenants)
                {
                    BGBC.Web.Models.TenantRentPay tRentPay = new BGBC.Web.Models.TenantRentPay();
                    tRentPay.tname = t.User.FirstName + " " + t.User.LastName;
                    tRentPay.RentPayment = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == t.User.UserID).OrderByDescending(d => d.TransDate).Take(5).ToList();
                    allProps.tenantRentPay.Add(tRentPay);
                }
                allPropertiesAndTenant.Add(allProps);
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
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(payment);
        }

    }
}