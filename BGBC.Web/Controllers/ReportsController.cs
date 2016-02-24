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
        IRepository<Order, int> _order;

        public ReportsController()
        {
            _contactForm = new ContactRepository();
            _tenantReferal = new TenantRefRepository();
            _tenantRepo = new TenantRepository();
            _pro = new PropertyRepository();
            _product = new ProductRepository();
            _userRepository = new UserRepository();
            _propertyRepo = new PropertyRepository();
            _order = new OrderRepository();
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
            int currentPageSize = int.Parse(PageSize == null ? "10" : PageSize), pageNumber = (page ?? 1);
            try
            {
                var products = (id == null ? BGBCFunctions.ProductOrders() : BGBCFunctions.ProductOrders().Where(x => x.ProductID == id));

                ViewBag.CurrentSort = sortOrder;
                ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
                ViewBag.OrderSortParm = sortOrder == "OrderID" ? "OrderID_desc" : "OrderID";
                ViewBag.CustomerSortParm = sortOrder == "Customer" ? "Customer_desc" : "Customer";
                ViewBag.TypeSortParm = sortOrder == "Type" ? "Type_desc" : "Type";
                ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";
                if (PageSize != null)
                    ViewBag.currentPageSize = currentPageSize;


                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;
                ViewBag.page = pageNumber;
                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(x => x.CustomerFName.Contains(searchString) || x.CustomerLName.Contains(searchString)
                        || x.Name.Contains(searchString) || x.TransId.Contains(searchString));

                }

                switch (sortOrder)
                {
                    case "date_desc":
                        products = products.OrderByDescending(x => x.TransDate);
                        break;
                    case "OrderID":
                        products = products.OrderBy(x => x.TransId);
                        break;
                    case "OrderID_desc":
                        products = products.OrderByDescending(x => x.TransId);
                        break;
                    case "Customer":
                        products = products.OrderBy(x => x.CustomerFName);
                        break;
                    case "Customer_desc":
                        products = products.OrderByDescending(x => x.CustomerFName);
                        break;
                    case "Type":
                        products = products.OrderBy(x => x.UserType);
                        break;
                    case "Type_desc":
                        products = products.OrderByDescending(x => x.UserType);
                        break;
                    case "Price":
                        products = products.OrderBy(x => x.Price);
                        break;
                    case "Price_desc":
                        products = products.OrderByDescending(x => x.Price);
                        break;
                    default:  // Date ascending 
                        products = products.OrderBy(x => x.TransDate);
                        break;
                }

                return View(products.ToPagedList(pageNumber, currentPageSize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();

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

        [Authorize]
        public ActionResult ProductInvoice(int id)
        {

            IEnumerable<ProductOrder> productOrderList = new List<ProductOrder>();
            try
            {
                Order order = _order.Get(id);
                ViewBag.TransactionNo = order.TransId;
                ViewBag.Date = order.TransDate;
                ViewBag.BillAddress = order.BillAddress;
                ViewBag.Customer = order.User.FirstName + " " + order.User.LastName;
                productOrderList = BGBCFunctions.ProductOrderIds().Where(x => x.OrderID == id).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View(productOrderList);
        }

    }
}