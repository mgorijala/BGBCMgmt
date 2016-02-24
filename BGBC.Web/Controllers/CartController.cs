using BGBC.Core;
using BGBC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BGBC.Web.Controllers
{
    public class CartController : Controller
    {
        private IRepository<Product, int?> _repository;
        private IUserRepository _userRepository;
        private IRepository<Profile, int> _profileRepo;
        public CartController()
        {
            _repository = new ProductRepository();
            _userRepository = new UserRepository();
            _profileRepo = new ProfileRepository();
        }
        public ActionResult Index()
        {
            HttpCookie authCookie = Request.Cookies[".BGBCProducts"];
            BGBC.Web.Models.Checkout checkout = new Models.Checkout();
            checkout.OrderList = new List<Models.OrderSummary>();
            checkout.OrderTotal = 0;
            if (authCookie != null)
            {
                string[] ids = authCookie.Value.ToString().Split(',');
                IEnumerable<Product> list = _repository.Get();
                string cookiesstring = string.Empty;
                foreach (var item in list)
                {
                    if (Array.Exists(ids, e => e == item.ProductID.ToString()))
                    {
                        checkout.OrderList.Add(new BGBC.Web.Models.OrderSummary { Item = item.Name, ProductID = item.ProductID, Price = (decimal)item.Price, Quantity = 1 });
                        checkout.OrderTotal = checkout.OrderTotal + (decimal)item.Price;
                        if (cookiesstring == string.Empty)
                        {
                            cookiesstring = cookiesstring + item.ProductID;
                        }
                        else
                        {
                            cookiesstring = cookiesstring + "," + item.ProductID;
                        }
                    }
                }
                authCookie.Value = cookiesstring;
                Response.SetCookie(authCookie);
            }
            return View(checkout);
        }

        public ActionResult Addtocart(int? id)
        {
            if (id != null)
            {
                HttpCookie authCookie = Request.Cookies[".BGBCProducts"];
                if (authCookie == null)
                {
                    HttpCookie faCookie = new HttpCookie(".BGBCProducts", id.ToString());
                    Response.Cookies.Add(faCookie);
                    TempData["SucessMessage"] = "Item Added to Cart";
                }
                else
                {
                    string[] ids = authCookie.Value.ToString().Split(',');
                    if (!Array.Exists(ids, e => e == id.ToString()))
                    {
                        {
                            authCookie.Value = authCookie.Value + (authCookie.Value.ToString().Length > 0 ? "," : "") + id.ToString();
                            Response.SetCookie(authCookie);
                            TempData["SucessMessage"] = "Item Added to Cart";
                        }
                    }
                    else { TempData["SucessMessage"] = "User has already added a product to the cart"; }
                }
            }
            return RedirectToAction("Index", "Products");
        }

        public ActionResult Removefromcart(int? id)
        {
            if (id != null)
            {
                HttpCookie authCookie = Request.Cookies[".BGBCProducts"];
                if (authCookie != null)
                {
                    string[] ids = authCookie.Value.ToString().Split(',');
                    if (Array.Exists(ids, e => e == id.ToString()))
                    {
                        ids = ids.Except(new string[] { id.ToString() }).ToArray();
                        authCookie.Value = string.Join(",", ids);
                        Response.SetCookie(authCookie);
                    }
                }
            }
            return RedirectToAction("Index", "Cart");
        }

        public ActionResult Checkout()
        {
            Models.Checkout checkout = new Models.Checkout();
            checkout.OrderList = new List<Models.OrderSummary>();
            if (TempData["cartdata"] != null)
            {
                checkout = (Models.Checkout)TempData["cartdata"];
                checkout.CVV = string.Empty;
            }

            try
            {
                if (Request.IsAuthenticated)
                {
                    User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                    Profile profile = user.Profiles.FirstOrDefault();
                    checkout.FirstName = user.FirstName;
                    checkout.LastName = user.LastName;
                    checkout.Email = user.Email;
                    checkout.BillingAddress = profile.BillingAddress;
                    checkout.BillingAddress_2 = profile.BillingAddress_2;
                    checkout.BillingCty = profile.BillingCty;
                    checkout.BillingState = profile.BillingState;
                    checkout.BillingZip = profile.BillingZip;
                    checkout.Phone = profile.MobilePhone;
                }

                Tuple<List<Models.OrderSummary>, decimal, string> ordersummary = getCartProducts();
                checkout.OrderList = ordersummary.Item1;
                checkout.OrderTotal = ordersummary.Item2;
                checkout.ProductIds = ordersummary.Item3;
            }
            catch (Exception ex)
            {

            }
            checkoutDropDown();
            return View(checkout);
        }

        [HttpPost]
        public ActionResult Checkoutconfirm(Models.Checkout checkout)
        {

            if (!Request.IsAuthenticated && (string.IsNullOrEmpty(checkout.ChoosePassword))) //For login user password is not required
            {
                ModelState.AddModelError("ChoosePassword", "The password field is required");
                ModelState.AddModelError("ConfirmPassword", "The password field is required");
            }


            if (string.IsNullOrEmpty(checkout.BillingAddress)) ModelState.AddModelError("BillingAddress", "The Billing Address field is required.");
            if (string.IsNullOrEmpty(checkout.BillingCty)) ModelState.AddModelError("BillingCty", "The Billing City field is required.");
            if (string.IsNullOrEmpty(checkout.BillingState)) ModelState.AddModelError("BillingState", "The Billing State field is required.");
            if (string.IsNullOrEmpty(checkout.BillingZip)) ModelState.AddModelError("BillingZip", "The Billing Zip field is required.");


            if (!checkout.ServiceBillingAddressSame)
            {
                if (string.IsNullOrEmpty(checkout.ServiceAddress)) ModelState.AddModelError("ServiceAddress", "The Service Address field is required.");
                if (string.IsNullOrEmpty(checkout.ServiceCty)) ModelState.AddModelError("ServiceCty", "The Service City field is required.");
                if (string.IsNullOrEmpty(checkout.ServiceState)) ModelState.AddModelError("ServiceState", "The Service State field is required.");
                if (string.IsNullOrEmpty(checkout.ServiceZip)) ModelState.AddModelError("ServiceZip", "The Service Zip field is required.");
            }



            if (checkout.PaymentMethod == "eCheck")
            {
                if (!string.IsNullOrEmpty(checkout.BankRoutingNumber))
                {
                    if (checkout.BankRoutingNumber.Trim().Length != 9)
                        ModelState.AddModelError("BankRoutingNumber", "Please enter a valid routing number");
                }
                else
                {
                    ModelState.AddModelError("BankRoutingNumber", "Please enter a valid routing number");
                }
                if (!string.IsNullOrEmpty(checkout.BankAccountNumber))
                {
                    if (checkout.BankAccountNumber.Trim().Length != 7)
                        ModelState.AddModelError("BankAccountNumber", "Please enter a valid account number");
                }
                else
                {
                    ModelState.AddModelError("BankAccountNumber", "Please enter a valid account number");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(checkout.CardNo)) ModelState.AddModelError("CardNo", "The Card No field is required.");
                if (string.IsNullOrEmpty(checkout.CVV)) ModelState.AddModelError("CVV", "The Card CVV field is required.");

                if (checkout.CardNo.Trim().Length > 0)
                {
                    if (CreditCardUtility.IsValidNumber(checkout.CardNo))
                    {
                        if (!string.IsNullOrEmpty(checkout.CVV))
                        {
                            CreditCardTypeType? cardType = CreditCardUtility.GetCardTypeFromNumber(checkout.CardNo);
                            if (cardType == null)
                            {
                                ModelState.AddModelError("CardNo", "Please enter a valid card number");
                            }
                            else
                            {
                                checkout.CardType = (CreditCardTypeType)cardType;
                                if (cardType == CreditCardTypeType.Amex && checkout.CVV.Trim().Length != 4) ModelState.AddModelError("CVV", "Please enter a valid CVV number");
                                else if (cardType != CreditCardTypeType.Amex && checkout.CVV.Trim().Length != 3) ModelState.AddModelError("CVV", "Please enter a valid CVV number");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("CardNo", "Please enter a valid card number");
                    }
                }
            }

            var query = from state in ModelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            var errorList = query.ToList();
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                if (!Request.IsAuthenticated)
                {
                    User user = _userRepository.Find(checkout.Email);
                    if (user != null)
                    {
                        ModelState.AddModelError("Email", "Email is already exists");
                    }
                    else
                    {
                        TempData["cartdata"] = checkout;
                        return View(checkout);
                    }
                }
                else
                {
                    TempData["cartdata"] = checkout;
                    return View(checkout);
                }
            }
            checkoutDropDown();
            return View("Checkout", checkout);
        }

        private Tuple<List<Models.OrderSummary>, decimal, string> getCartProducts()
        {
            List<Models.OrderSummary> OrderList = new List<Models.OrderSummary>();
            HttpCookie authCookie = Request.Cookies[".BGBCProducts"];
            List<Product> cartPro = new List<Product>();
            decimal total = 0;
            if (authCookie != null)
            {
                string[] ids = authCookie.Value.ToString().Split(',');
                IEnumerable<Product> list = _repository.Get();
                foreach (var item in list)
                {
                    if (Array.Exists(ids, e => e == item.ProductID.ToString()))
                    {
                        OrderList.Add(new Models.OrderSummary { ProductID = item.ProductID, Item = item.Name, Price = (decimal)item.Price, Quantity = 1, Subtotal = (decimal)item.Price, rowtype = "product" });
                        total = total + (decimal)item.Price;
                    }

                }
                if (OrderList.Count > 0) //Add tax and Processing/Convenience Fee 
                {
                    decimal fee = Math.Round((total * (8m / 100)), 2);
                    decimal tax = Math.Round((total * (8.25m / 100)), 2);
                    OrderList.Add(new Models.OrderSummary { ProductID = 0, Item = "Payment Processing and Convenience Fee (8%)", Price = fee, Quantity = 1, Subtotal = fee, rowtype = "fee" });
                    OrderList.Add(new Models.OrderSummary { ProductID = 0, Item = "Tax (8.25%)", Price = tax, Quantity = 1, Subtotal = tax, rowtype = "tax" });
                    total = total + tax + fee;
                }
            }
            return new Tuple<List<Models.OrderSummary>, decimal, string>(OrderList, total, authCookie.Value.ToString());
        }

        [HttpPost]
        public ActionResult Transaction()
        {
            Models.Checkout checkout = new Models.Checkout();
            try
            {
                checkout = (Models.Checkout)TempData["cartdata"];
                var lineItems = new AuthorizeNet.Api.Contracts.V1.lineItemType[checkout.OrderList.Count];
                for (int i = 0; i < checkout.OrderList.Count; i++)
                {
                    lineItems[i] = new AuthorizeNet.Api.Contracts.V1.lineItemType { itemId = i.ToString(), name = (checkout.OrderList[i].Item.Length < 10) ? checkout.OrderList[i].Item : checkout.OrderList[i].Item.Substring(0, 10), quantity = checkout.OrderList[i].Quantity, unitPrice = checkout.OrderList[i].Subtotal };
                }

                int invoiceNumber = BGBCFunctions.GetInoiveNo();
                AuthorizeNet.Api.Controllers.createTransactionController controller;
                var billAddress = new AuthorizeNet.Api.Contracts.V1.customerAddressType { firstName = checkout.FirstName, lastName = checkout.LastName, address = checkout.BillingAddress + ", " + (string.IsNullOrEmpty(checkout.BillingAddress_2) ? "" : checkout.BillingAddress_2), city = checkout.BillingCty, state = checkout.BillingState, zip = checkout.BillingZip, email = checkout.Email, phoneNumber = checkout.Phone, country = "USA" };
                string address = string.Format("{0}<br/>{1}<br/>{2}, {3} {4}", checkout.BillingAddress, checkout.BillingAddress_2, checkout.BillingCty, checkout.BillingState, checkout.BillingZip);
                if (checkout.PaymentMethod == "eCheck")
                {
                    var bankAccount = new AuthorizeNet.Api.Contracts.V1.bankAccountType
                    {
                        accountNumber = checkout.BankAccountNumber,
                        routingNumber = checkout.BankRoutingNumber,
                        echeckType = AuthorizeNet.Api.Contracts.V1.echeckTypeEnum.WEB,   // change based on how you take the payment (web, telephone, etc)
                        nameOnAccount = checkout.FirstName + " " + checkout.LastName
                    };
                    controller = PaymentGateway.DebitBankAccount(bankAccount, lineItems, billAddress, checkout.OrderTotal, invoiceNumber);


                }
                else
                {
                    var creditCard = new AuthorizeNet.Api.Contracts.V1.creditCardType
                    {
                        cardNumber = checkout.CardNo,
                        expirationDate = checkout.CardExpMon + checkout.CardExpYear.ToString(),
                        cardCode = checkout.CVV
                    };
                    controller = PaymentGateway.ChargeCreditCard(creditCard, lineItems, billAddress, checkout.OrderTotal, invoiceNumber);
                }
                AuthorizeNet.Api.Contracts.V1.createTransactionResponse response = controller.GetApiResponse();

                if (response != null)
                {
                    if (response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse != null)
                        {
                            try
                            {
                                int userid = 0;
                                if (!Request.IsAuthenticated)
                                {
                                    User selUser = new User { FirstName = checkout.FirstName, LastName = checkout.LastName, Password = BGBC.Core.Security.Cryptography.Encrypt(checkout.ChoosePassword), UserType = 3, Email = checkout.Email };
                                    selUser.Profiles.Add(new Profile
                                    {
                                        BillingAddress = checkout.BillingAddress,
                                        BillingAddress_2 = checkout.BillingAddress_2,
                                        BillingCty = checkout.BillingCty,
                                        BillingState = checkout.BillingState,
                                        MobilePhone = checkout.Phone
                                    });

                                    selUser = _userRepository.Add(selUser);
                                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                                    serializeModel.UserId = selUser.UserID;
                                    serializeModel.FirstName = selUser.FirstName;
                                    serializeModel.LastName = selUser.LastName;
                                    serializeModel.roles = new string[] { "Customer" };

                                    string userData = Newtonsoft.Json.JsonConvert.SerializeObject(serializeModel);
                                    System.Web.Security.FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, selUser.FirstName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);

                                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                                    Response.Cookies.Add(faCookie);
                                    userid = selUser.UserID;
                                }
                                else
                                {
                                    userid = ((BGBC.Core.CustomPrincipal)(User)).UserId;
                                }
                                Order order = BGBCFunctions.ProductTrans(userid, checkout.Email, checkout.ChoosePassword, invoiceNumber,
                                    response.transactionResponse.accountNumber, response.transactionResponse.accountType, response.transactionResponse.transId,
                                    response.transactionResponse.messages[0].code, response.transactionResponse.messages[0].description, Request.UserHostAddress, address, checkout.Comments,
                                    checkout.ProductIds);

                                IRepository<ProductOrder, int> productOrder = new ProductOrderRepository();
                                foreach (var item in checkout.OrderList.Where(x => x.ProductID == 0))
                                {
                                    productOrder.Add(new ProductOrder { OrderID = order.OrderID, Name = item.Item, Price = item.Price });
                                }

                                HttpCookie authCookie = Request.Cookies[".BGBCProducts"];
                                authCookie.Value = string.Empty;
                                Response.SetCookie(authCookie);

                                return RedirectToAction("OrderHistory", "Reports");
                            }
                            catch (Exception ex) { ModelState.AddModelError("", "Transaction Error : " + ex.Message); }
                            System.Diagnostics.Trace.TraceInformation("Success, Auth Code : " + response.transactionResponse.authCode);
                        }
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceInformation("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                        if (response.transactionResponse != null)
                        {
                            ModelState.AddModelError("", "Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText);
                        }
                        TempData["cartdata"] = checkout;
                    }
                }
                else
                {
                    TempData["cartdata"] = checkout;
                    ModelState.AddModelError("", "Transaction Error, unable to complete the transaction.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Transaction Error, unable to complete the transaction.");
            }
            Tuple<List<Models.OrderSummary>, decimal, string> ordersummary = getCartProducts();
            checkout.OrderList = ordersummary.Item1;
            checkout.OrderTotal = ordersummary.Item2;
            checkout.ProductIds = ordersummary.Item3;
            checkoutDropDown();
            return View("Checkout", checkout);
        }

        private void checkoutDropDown()
        {
            List<SelectListItem> months = new List<SelectListItem>();
            List<SelectListItem> years = new List<SelectListItem>();
            for (int i = 1; i <= 12; i++)
            {
                months.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString().PadLeft(2, '0') });
            }
            for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++)
            {
                years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString().Substring(2, 2) });
            }

            ViewBag.Months = months;
            ViewBag.Years = years;

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