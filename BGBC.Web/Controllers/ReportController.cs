using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using BGBC.Core;

namespace BGBC.Web.Controllers
{
    public class ReportController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ReportController));
        private IRepository<ContactForm, int> _contactForm;
        private IRepository<TenantReferral, int?> _tenantReferal;
        private IRepository<Tenant, int> _tenantRepo;
        private IRepository<BGBC.Model.Property, int> _pro;
        private IRepository<Product, int?> _product;
        IUserRepository _userRepository;
        IRepository<Property, int> _propertyRepo;
        IRepository<Order, int> _order;
        IRepository<Profile, int> _profileRepo;
        IRepository<UserCart, int?> _userCart;
        IRepository<Email, int?> _email;

        public ReportController()
        {
            _email = new EmailRepository();
            _contactForm = new ContactRepository();
            _tenantReferal = new TenantRefRepository();
            _tenantRepo = new TenantRepository();
            _pro = new PropertyRepository();
            _product = new ProductRepository();
            _userRepository = new UserRepository();
            _propertyRepo = new PropertyRepository();
            _order = new OrderRepository();
            _profileRepo = new ProfileRepository();
            _userCart = new UserCartRepository();
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Contact()
        {
            return View(_contactForm.GetRef(1));
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult ContactRealtor()
        {
            return View(_contactForm.GetRef(1));
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult TenantReferal()
        {
            return View(_tenantReferal.Get());
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PayoutPreferences(string sortOrder, string currentFilter, string searchString, int? page, string pageSize)
        {


            int currentPageSize = int.Parse(pageSize == null ? "10" : pageSize), pageNumber = (page ?? 1);
            try
            {
                var profile = _profileRepo.Get().Where(x => x.User.Deletedon == null && x.User.UserType == 1);

                ViewBag.currentSort = sortOrder;
                ViewBag.nameSortParam = sortOrder == "name" ? "name_desc" : "name";
                ViewBag.paymentMethodSortParam = sortOrder == "payment_method" ? "payment_method_desc" : "payment_method";
                ViewBag.paypalEmailSortParam = sortOrder == "paypal_email" ? "paypal_email_desc" : "paypal_email";
                ViewBag.paypalEmailAddressSortParam = sortOrder == "paypal_email_address" ? "paypal_email_address_desc" : "paypal_email_address";
                ViewBag.paypalEmailAddress2SortParam = sortOrder == "paypal_email_address2" ? "paypal_email_address2_desc" : "paypal_email_address2";
                ViewBag.payoutMailingCitySortParam = sortOrder == "payout_mailing_city" ? "payout_mailing_city_desc" : "payout_mailing_city";
                ViewBag.payoutMailingStateSortParam = sortOrder == "payout_mailing_state" ? "payout_mailing_state_desc" : "payout_mailing_state";
                ViewBag.payoutMailingZipSortParam = sortOrder == "payout_mailing_zip" ? "payout_mailing_zip_desc" : "payout_mailing_zip";

                if (pageSize != null)
                    ViewBag.currentPageSize = currentPageSize;


                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.currentFilter = searchString;
                ViewBag.page = pageNumber;
                if (!string.IsNullOrEmpty(searchString))
                {
                    profile = profile.Where(x => x.User.FirstName.Contains(searchString) || x.PaymentMethod.Contains(searchString) || x.PaypalEmail.Contains(searchString)
                        || x.PayoutMailAddress.Contains(searchString) || x.PayoutMailAddress2.Contains(searchString) || x.PayoutMailCity.Contains(searchString)
                        || x.PayoutMailState.Contains(searchString) || x.PayoutMailZip.Contains(searchString));
                }
                switch (sortOrder)
                {
                    case "name_desc":
                        profile = profile.OrderByDescending(x => x.User.FirstName);
                        break;
                    case "payment_method":
                        profile = profile.OrderBy(x => x.PaymentMethod);
                        break;
                    case "payment_method_desc":
                        profile = profile.OrderByDescending(x => x.PaymentMethod);
                        break;
                    case "paypal_email":
                        profile = profile.OrderBy(x => x.PaypalEmail);
                        break;
                    case "paypal_email_desc":
                        profile = profile.OrderByDescending(x => x.PaypalEmail);
                        break;

                    case "paypal_email_address":
                        profile = profile.OrderBy(x => x.PayoutMailAddress);
                        break;
                    case "paypal_email_address_desc":
                        profile = profile.OrderByDescending(x => x.PayoutMailAddress);
                        break;
                    case "paypal_email_address2":
                        profile = profile.OrderBy(x => x.PayoutMailAddress2);
                        break;
                    case "paypal_email_address2_desc":
                        profile = profile.OrderByDescending(x => x.PayoutMailAddress2);
                        break;
                    case "payout_mailing_city":
                        profile = profile.OrderBy(x => x.PayoutMailCity);
                        break;
                    case "payout_mailing_city_desc":
                        profile = profile.OrderByDescending(x => x.PayoutMailCity);
                        break;
                    case "payout_mailing_state":
                        profile = profile.OrderBy(x => x.PayoutMailState);
                        break;
                    case "payout_mailing_state_desc":
                        profile = profile.OrderByDescending(x => x.PayoutMailState);
                        break;
                    case "payout_mailing_zip":
                        profile = profile.OrderBy(x => x.PayoutMailZip);
                        break;
                    case "payout_mailing_zip_desc":
                        profile = profile.OrderByDescending(x => x.PayoutMailZip);
                        break;
                    default:
                        profile = profile.OrderBy(x => x.User.FirstName);
                        break;
                }

                return View(profile.ToPagedList(pageNumber, currentPageSize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();

        }

        [Authorize]
        public ActionResult AllProductsOrders(int? id, string sortOrder, string currentFilter, string searchString, int? page, string PageSize)
        {
            if (id != null)
            {
                Product product = _product.Get(id);
                ViewBag.Name = product.Name;
            }
            int currentPageSize = int.Parse(PageSize == null ? "10" : PageSize), pageNumber = (page ?? 1);
            try
            {
                var products = (id == null ? BGBCFunctions.ProductOrders() : BGBCFunctions.ProductOrders().Where(x => x.ProductID == id));

                ViewBag.CurrentSort = sortOrder;
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.TransIDSortParm = sortOrder == "TransID" ? "TransID_desc" : "TransID";
                ViewBag.CustomerSortParm = sortOrder == "Customer" ? "Customer_desc" : "Customer";
                ViewBag.TypeSortParm = sortOrder == "Type" ? "Type_desc" : "Type";
                ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";
                ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
                ViewBag.CommentsSortParm = sortOrder == "Comments" ? "comments_desc" : "Comments";
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
                    case "TransID":
                        products = products.OrderBy(x => x.TransId);
                        break;
                    case "TransID_desc":
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
                    case "Name":
                        products = products.OrderBy(x => x.Name);
                        break;
                    case "name_desc":
                        products = products.OrderByDescending(x => x.Name);
                        break;
                    case "Price":
                        products = products.OrderBy(x => x.Price);
                        break;
                    case "Price_desc":
                        products = products.OrderByDescending(x => x.Price);
                        break;
                    case "Comments":
                        products = products.OrderBy(x => x.Comments);
                        break;
                    case "comments_desc":
                        products = products.OrderByDescending(x => x.Comments);
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

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult PaymentHistory(string sortOrder, string currentFilter, string searchString, int? page, string pageSize,string searchOwner)
        {
            int currentPageSize = int.Parse(pageSize == null ? "10" : pageSize), pageNumber = (page ?? 1);
            try
            {
                var rentPayments = BGBCFunctions.RentPayments();
                ViewBag.currentSort = sortOrder;
                ViewBag.dateSortParam = sortOrder == "date" ? "date_desc" : "date";
                ViewBag.idSortParam = sortOrder == "id" ? "id_desc" : "id";
                ViewBag.nameSortParam = sortOrder == "name" ? "name_desc" : "name";
                ViewBag.ownerSortParam = sortOrder == "owner" ? "owner_desc" : "owner";
                ViewBag.descSortParam = sortOrder == "desc" ? "desc_desc" : "desc";
                ViewBag.amountSortParam = sortOrder == "amount" ? "amount_desc" : "amount";
                ViewBag.commentSortParam = sortOrder == "comment" ? "comment_desc" : "comment";

                if (pageSize != null)
                    ViewBag.currentPageSize = currentPageSize;


                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.currentFilter = searchString;
                ViewBag.page = pageNumber;
                if (!string.IsNullOrEmpty(searchString))
                {
                    rentPayments = rentPayments.Where(x => x.TenantLastName.Contains(searchString) || x.TenantFirstName.Contains(searchString)
                        || x.OwnerFirstName.Contains(searchString) || x.OwnerLastName.Contains(searchString)
                        || x.TransID.Contains(searchString));
                    //  ||  x.Amount.Equals(searchString)||  x.TransDate.Equals(searchString));
                }
                if (!string.IsNullOrEmpty(searchOwner))
                    rentPayments.Where(x => x.OwnerFirstName.Contains(searchOwner) || x.OwnerLastName.Contains(searchOwner));
                switch (sortOrder)
                {
                    case "date_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.TransDate);
                        break;
                    case "id":
                        rentPayments = rentPayments.OrderBy(x => x.ID);
                        break;
                    case "id_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.ID);
                        break;
                    case "name":
                        rentPayments = rentPayments.OrderBy(x => x.TenantFirstName);
                        break;
                    case "name_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.TenantFirstName);
                        break;

                    case "owner":
                        rentPayments = rentPayments.OrderBy(x => x.OwnerFirstName);
                        break;
                    case "owner_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.OwnerFirstName);
                        break;
                    case "desc":
                        rentPayments = rentPayments.OrderBy(x => x.Description);
                        break;
                    case "desc_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.Description);
                        break;
                    case "amount":
                        rentPayments = rentPayments.OrderBy(x => x.Amount);
                        break;
                    case "amount_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.Amount);
                        break;
                    case "comment":
                        rentPayments = rentPayments.OrderBy(x => x.Comments);
                        break;
                    case "comment_desc":
                        rentPayments = rentPayments.OrderByDescending(x => x.Comments);
                        break;
                    default:
                        rentPayments = rentPayments.OrderBy(x => x.TransDate);
                        break;
                }
                PropagateOwnerDropdown();
                return View(rentPayments.ToPagedList(pageNumber, currentPageSize));
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

        [Authorize]
        public ActionResult TenantPaymentHistory(int? id)
        {
            IEnumerable<vRentPayment> paymentdetails = new List<vRentPayment>();
            try
            {
                paymentdetails = BGBCFunctions.RentPayments()
                    .Where(x => x.TenantUserID == (id == null ? ((BGBC.Core.CustomPrincipal)(User)).UserId : id))
                    .OrderByDescending(d => d.TransDate).ToList();

                Tenant tenants = _tenantRepo.Get().Where(x => x.UserID == (id == null ? ((BGBC.Core.CustomPrincipal)(User)).UserId : id)).FirstOrDefault();

                ViewBag.Propertyname = (tenants != null ? tenants.Property.Name : "");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View(paymentdetails);
        }

        [Authorize]
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

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult UserCartHistory(string sortOrder, string currentFilter, string searchString, int? page, string pageSize)
        {

            int currentPageSize = int.Parse(pageSize == null ? "10" : pageSize), pageNumber = (page ?? 1);
            try
            {
                var userCart = _userCart.Get().Where(x => x.Deletedon == null);

                ViewBag.currentSort = sortOrder;
                ViewBag.dateSortParam = sortOrder == "date" ? "date_desc" : "date";
                ViewBag.productNameSortParam = sortOrder == "product_name" ? "product_name_desc" : "product_name";
                ViewBag.userNameSortParam = sortOrder == "user_name" ? "user_name_desc" : "user_name";
                ViewBag.emailSortParam = sortOrder == "email" ? "email_desc" : "email";
                ViewBag.contactParam = sortOrder == "contact" ? "contact_desc" : "contact";


                if (pageSize != null)
                    ViewBag.currentPageSize = currentPageSize;


                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.currentFilter = searchString;
                ViewBag.page = pageNumber;
                if (!string.IsNullOrEmpty(searchString))
                {
                    userCart = userCart.Where(x => x.User.FirstName.Contains(searchString) || x.User.LastName.Contains(searchString) || x.User.Email.Contains(searchString)
                        || x.Product.Name.Contains(searchString));
                    //pending date and contactno
                }
                switch (sortOrder)
                {
                    case "date_desc":
                        userCart = userCart.OrderByDescending(x => x.Createdon);
                        break;
                    case "product_name":
                        userCart = userCart.OrderBy(x => x.Product.Name);
                        break;
                    case "product_name_desc":
                        userCart = userCart.OrderByDescending(x => x.Product.Name);
                        break;
                    case "user_name":
                        userCart = userCart.OrderBy(x => x.User.FirstName);
                        break;
                    case "user_name_desc":
                        userCart = userCart.OrderByDescending(x => x.User.FirstName);
                        break;

                    case "email":
                        userCart = userCart.OrderBy(x => x.User.Email);
                        break;
                    case "email_desc":
                        userCart = userCart.OrderByDescending(x => x.User.Email);
                        break;
                    //case "contact":
                    //    userCart = userCart.OrderBy(x => x.PayoutMailAddress2);
                    //    break;
                    //case "contact_desc":
                    //    userCart = userCart.OrderByDescending(x => x.PayoutMailAddress2);
                    //    break;
                    default:
                        userCart = userCart.OrderBy(x => x.Createdon);
                        break;
                }

                return View(userCart.ToPagedList(pageNumber, currentPageSize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }


        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult Mails(int? id, string sortOrder, string currentFilter, string searchString, int? page, string PageSize, DateTime? fromdate, DateTime? todate)
        {

            int currentPageSize = int.Parse(PageSize == null ? "10" : PageSize), pageNumber = (page ?? 1);
            try
            {
                var emails = (id == null ? BGBCFunctions.EmailDates() : BGBCFunctions.EmailDates().Where(x => x.EmailID == id));
                if (PageSize != null)
                    ViewBag.currentPageSize = currentPageSize;

                ViewBag.CurrentSort = sortOrder;
                ViewBag.toSortParm = sortOrder == "To_Address" ? "to_desc" : "To_Address";
                ViewBag.subSortParm = sortOrder == "Subject" ? "sub_desc" : "Subject";
                ViewBag.bodySortParm = sortOrder == "Body" ? "body_desc" : "Body";
                ViewBag.dateSortParm = sortOrder == "Mail_Date" ? "date_desc" : "Mail_Date";

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
                if (fromdate != null)
                {
                    ViewBag.fromdate = fromdate;
                    ViewBag.todate = todate;
                }

                if (fromdate.HasValue)
                    emails = emails.Where(x => x.Createdon >= fromdate);
                if (todate.HasValue)
                {
                    var finaldate = Convert.ToDateTime(todate).AddDays(1);
                    emails = emails.Where(x => x.Createdon <= finaldate);
                }
                if (!string.IsNullOrEmpty(searchString))
                    emails = emails.Where(x => x.ToAddress.Contains(searchString) || x.Subject.Contains(searchString));

                switch (sortOrder)
                {
                    case "to_desc":
                        emails = emails.OrderBy(x => x.ToAddress);
                        break;
                    case "sub_desc":
                        emails = emails.OrderBy(x => x.Subject);
                        break;
                    case "body_desc":
                        emails = emails.OrderBy(x => x.Body);
                        break;
                    case "date_desc":
                        emails = emails.OrderBy(x => x.Createdon);
                        break;
                    default:
                        emails = emails.OrderBy(x => x.Createdon);
                        break;
                }
                return View(emails.ToPagedList(pageNumber, currentPageSize));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return View();
        }

        void PropagateOwnerDropdown()
        {
            var rentPayments = _propertyRepo.Get().Select(x => new {x.User.FirstName,x.User.LastName }).Distinct(); 
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in rentPayments)
            {
                items.Add(new SelectListItem
                {
                    Text = Convert.ToString(item.FirstName + " " + item.LastName),
                    Value = Convert.ToString(item.FirstName)
                });
            }
            ViewBag.OwnerList = items;
        }
    }
}