using BGBC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BGBC.Model;
using BGBC.Web.Models;
using System.Net;
using BGBC.Model.Metadata;
using System.Text.RegularExpressions;
using BGBC.Core.Security;
using System.Web.Routing;

namespace BGBC.Web.Controllers
{
    public class TenantController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TenantController));
        private IRepository<Tenant, int> _tenantRepo;
        IUserRepository _userRepository;
        IRepository<Profile, int> _profileRepo;
        IRepository<UserReference, int> _userRefRepo;
        IRepository<Property, int> _propertyRepo;
        IRepository<RentDue, int?> _rentDueRepo;
        IRepository<PasswordReset, int?> passwordresetRepo;
        IRepository<UserCC, int> _userCCRep;
        IRepository<RentAutoPay, int> _rentAutoPayRep;

        public TenantController()
        {
            _tenantRepo = new TenantRepository();
            _userRepository = new UserRepository();
            _profileRepo = new ProfileRepository();
            _userRefRepo = new UserReferenceRepository();
            _propertyRepo = new PropertyRepository();
            _rentDueRepo = new RentDueRepository();
            passwordresetRepo = new PasswordResetRepository();
            _userCCRep = new UserCCRepository();
            _rentAutoPayRep = new RentAutoPayRepository();
        }

        [CustomAuthorize(Roles = "Tenant")]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [CustomAuthorize(Roles = "Tenant")]
        public ActionResult MyAccount()
        {
            Models.MyAccount myaccount = new MyAccount();

            myaccount.TenantRent = new List<Models.Rentdue>();
            myaccount.RecentPayments = new List<Models.RecentPay>();
            try
            {
                User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);

                Property property = _propertyRepo.Get((int)user.Tenants.FirstOrDefault().PropertyID);
                myaccount.Name = property.User.FirstName;
                myaccount.Email = property.User.Email;
                myaccount.Phone = property.User.Profiles.FirstOrDefault().MobilePhone;
                myaccount.RentAmount = user.Tenants.FirstOrDefault().RentAmount;
                myaccount.RentDue = property.RentDueDay.ToString();
                List<RentDue> rentDue = _rentDueRepo.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId);
                foreach (var item in rentDue)
                {
                    myaccount.Select = true;
                    myaccount.TenantRent.Add(new Rentdue { ID = item.RentDueID, DueDate = (DateTime)item.DueDate, AmountDue = item.DueAmount, Charge = item.Description, select = false });
                }
                List<vRentPayment> paymentdetails = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == ((BGBC.Core.CustomPrincipal)(User)).UserId).OrderByDescending(d => d.TransDate).Take(5).ToList();
                foreach (var item in paymentdetails)
                {
                    myaccount.RecentPayments.Add(new RecentPay { Date = item.TransDate, Conformation = item.TransID, Description = item.Description, PaymentStatus = item.StatusText, PaymentAmount = item.Amount, Comment = item.Comments });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View(myaccount);
        }

        [HttpPost]
        [Authorize]
        public ActionResult MyAccount(MyAccount myAccount)
        {
            List<Rentdue> rentdue = new List<Rentdue>();
            if (myAccount.TenantRent != null)
            {
                foreach (var item in myAccount.TenantRent.Where(x => x.select == true))
                {
                    rentdue.Add(item);
                }
                if (rentdue.Count == 0)
                {
                    ModelState.AddModelError("", "Must select at least one charge to make a payment.");
                    return View();
                }
                TempData["rentdue"] = rentdue;
                TempData.Remove("data");
                return RedirectToAction("TenantPayment");
            }
            ModelState.AddModelError("", "Must select at least one charge to make a payment.");
            return View(myAccount);
        }

        [Authorize]
        public ActionResult TenantPayment()
        {
            if (TempData["rentdue"] == null && TempData["data"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Payments payments = new Models.Payments();
            payments.TenantRent = new List<Rentdue>();

            try
            {
                List<Rentdue> rentdue = (TempData["rentdue"] != null ? (List<Rentdue>)TempData["rentdue"] : new List<Rentdue>());
                TempData.Remove("rentdue");
                if (TempData["data"] != null)
                {
                    payments = (Models.Payments)TempData["data"];
                    payments.CVV = string.Empty;
                    rentdue = payments.TenantRent;
                }
                else
                {
                    User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);

                    Property property = _propertyRepo.Get((int)user.Tenants.FirstOrDefault().PropertyID);
                    Profile profile = user.Profiles.FirstOrDefault();
                    payments.PropertyID = property.PropertyID;
                    payments.FirstName = user.FirstName;
                    payments.LastName = user.LastName;
                    payments.Email = user.Email;
                    payments.BillingAddress = profile.BillingAddress;
                    payments.BillingAddress_2 = profile.BillingAddress_2;
                    payments.BillingCty = profile.BillingCty;
                    payments.BillingState = profile.BillingState;
                    payments.BillingZip = profile.BillingZip;
                    payments.Phone = profile.MobilePhone;
                    payments.OrderTotal = 0;
                    payments.UsePropertyAddress = true;
                    payments.PaymentMethod = "Credit Card";
                    List<RentDue> rentDue = _rentDueRepo.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId);

                    foreach (var item in rentDue)
                    {
                        if (rentdue.Where(x => x.ID == item.RentDueID && x.select == true).Count() > 0)
                        {
                            payments.TenantRent.Add(new Rentdue { ID = item.RentDueID, DueDate = (DateTime)item.DueDate, AmountDue = item.DueAmount, Charge = item.Description, PropertyID = property.PropertyID });
                            if (((DateTime)item.DueDate).Date.AddDays((short)property.GracePeriod) < DateTime.Today)
                            {
                                //Late Fee
                                payments.TenantRent.Add(new Rentdue { ID = 0, DueDate = DateTime.Today, AmountDue = (property.Penalty == null ? 0 : (decimal)property.Penalty), Charge = (item.Description + " Late Fee"), PropertyID = property.PropertyID });
                                payments.OrderTotal = payments.OrderTotal + (property.Penalty == null ? 0 : (decimal)property.Penalty);
                                //Daily Late Fees
                                int noDays = (DateTime.Today - ((DateTime)item.DueDate).Date.AddDays((short)property.GracePeriod)).Days;
                                payments.TenantRent.Add(new Rentdue { ID = 0, DueDate = DateTime.Today, AmountDue = Math.Round((property.DailyPenalty == null ? 0 : (decimal)property.DailyPenalty) * noDays, 2), Charge = (item.Description + string.Format(" Daily Late Fees (${0} x {1})", property.DailyPenalty, noDays)), PropertyID = property.PropertyID });
                                payments.OrderTotal = payments.OrderTotal + Math.Round((property.DailyPenalty == null ? 0 : (decimal)property.DailyPenalty) * noDays, 2);
                            }
                            payments.OrderTotal = payments.OrderTotal + item.DueAmount;
                        }
                    }
                    if (payments.TenantRent.Count > 0) //Add tax and Processing/Convenience Fee 
                    {
                        decimal fee = Math.Round((payments.OrderTotal * (8m / 100)), 2);
                        payments.TenantRent.Add(new Models.Rentdue { ID = 0, DueDate = DateTime.Today, Charge = "Payment Processing and Convenience Fee (8%)", AmountDue = fee, PropertyID = property.PropertyID });
                        payments.OrderTotal = payments.OrderTotal + fee;
                    }

                    UserCC ccinfo = _userCCRep.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                    if (ccinfo != null)
                    {
                        if (ccinfo.PaymentType == 1)
                        {
                            payments.PaymentMethod = "Credit Card"; payments.CardNo = Cryptography.Decrypt(ccinfo.CCNO); payments.CardExpMon = Cryptography.Decrypt(ccinfo.ExpMon);
                            payments.CardExpYear = Cryptography.Decrypt(ccinfo.ExpYear);
                        }
                        else { payments.PaymentMethod = "eCheck"; payments.BankRoutingNumber = Cryptography.Decrypt(ccinfo.RoutingNo); payments.BankAccountNumber = Cryptography.Decrypt(ccinfo.AccountNo); payments.BankAccountType = ccinfo.AccountType; }
                        payments.SaveCard = true;
                    }


                    TempData["data"] = payments;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            PopulateDropDown();
            return View(payments);
        }


        [HttpPost]
        [Authorize]
        public ActionResult TenantPayment(Payments payments)
        {
            if (!payments.UsePropertyAddress)
            {
                if (string.IsNullOrEmpty(payments.BillingAddress)) ModelState.AddModelError("BillingAddress", "The Billing Address field is required.");
                if (string.IsNullOrEmpty(payments.BillingCty)) ModelState.AddModelError("BillingCty", "The Billing City field is required.");
                if (string.IsNullOrEmpty(payments.BillingState)) ModelState.AddModelError("BillingState", "The Billing State field is required.");
                if (string.IsNullOrEmpty(payments.BillingZip)) ModelState.AddModelError("BillingZip", "The Billing Zip field is required.");
            }

            if (payments.PaymentMethod == "eCheck")
            {
                if (string.IsNullOrEmpty(payments.BankAccountType))
                {
                    ModelState.AddModelError("BankAccountType", "The Bank Account Type field is required.");
                }

                if (!string.IsNullOrEmpty(payments.BankRoutingNumber))
                {
                    if (payments.BankRoutingNumber.Trim().Length != 9)
                        ModelState.AddModelError("BankRoutingNumber", "Please enter a valid routing number");
                }
                else
                {
                    ModelState.AddModelError("BankRoutingNumber", "Please enter a valid routing number");
                }
                if (!string.IsNullOrEmpty(payments.BankAccountNumber))
                {
                    if (payments.BankAccountNumber.Trim().Length != 7)
                        ModelState.AddModelError("BankAccountNumber", "Please enter a valid account number");
                }
                else
                {
                    ModelState.AddModelError("BankAccountNumber", "Please enter a valid account number");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(payments.CardNo)) ModelState.AddModelError("CardNo", "The Card No field is required.");
                if (string.IsNullOrEmpty(payments.CVV)) ModelState.AddModelError("CVV", "The Card CVV field is required.");

                if (!string.IsNullOrEmpty(payments.CardNo))
                {
                    if (payments.CardNo.Trim().Length > 0)
                    {
                        if (CreditCardUtility.IsValidNumber(payments.CardNo))
                        {
                            if (!string.IsNullOrEmpty(payments.CVV))
                            {
                                CreditCardTypeType? cardType = CreditCardUtility.GetCardTypeFromNumber(payments.CardNo);
                                if (cardType == null)
                                {
                                    ModelState.AddModelError("CardNo", "Please enter a valid card number");
                                }
                                else
                                {
                                    payments.CardType = (CreditCardTypeType)cardType;
                                    if (cardType == CreditCardTypeType.Amex && payments.CVV.Trim().Length != 4) ModelState.AddModelError("CVV", "Please enter a valid CVV number");
                                    else if (cardType != CreditCardTypeType.Amex && payments.CVV.Trim().Length != 3) ModelState.AddModelError("CVV", "Please enter a valid CVV number");
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("CardNo", "Please enter a valid card number");
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Property property = _propertyRepo.Get(payments.PropertyID);
                    payments.BillingAddress = property.Address;
                    payments.BillingAddress_2 = property.Address2;
                    payments.BillingCty = property.City;
                    payments.BillingState = property.State;
                    payments.BillingZip = property.Zip;
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                TempData["data"] = payments;
                return View("TenantPaymentconfirm", payments);
            }
            PopulateDropDown();
            return View(payments);
        }

        [Authorize]
        public ActionResult Transaction()
        {
            Models.Payments payments = new Payments();
            try
            {
                payments = (Payments)TempData["data"];
                var lineItems = new AuthorizeNet.Api.Contracts.V1.lineItemType[payments.TenantRent.Count];
                int[] rentidsarry = new int[payments.TenantRent.Count];
                for (int i = 0; i < payments.TenantRent.Count; i++)
                {
                    lineItems[i] = new AuthorizeNet.Api.Contracts.V1.lineItemType { itemId = payments.TenantRent[i].ID.ToString(), name = (payments.TenantRent[i].Charge.Length < 10) ? payments.TenantRent[i].Charge : payments.TenantRent[i].Charge.Substring(0, 10), quantity = 1, unitPrice = payments.TenantRent[i].AmountDue };
                    rentidsarry[i] = payments.TenantRent[i].ID;
                }

                int invoiceNumber = BGBCFunctions.GetInoiveNo();
                AuthorizeNet.Api.Controllers.createTransactionController controller;
                var billAddress = new AuthorizeNet.Api.Contracts.V1.customerAddressType { firstName = payments.FirstName, lastName = payments.LastName, address = payments.BillingAddress + ", " + (string.IsNullOrEmpty(payments.BillingAddress_2) ? "" : payments.BillingAddress_2), city = payments.BillingCty, state = payments.BillingState, zip = payments.BillingZip, email = payments.Email, phoneNumber = payments.Phone, country = "USA" };
                string address = string.Format("{0}<br/>{1}<br/>{2}, {3} {4}", payments.BillingAddress, payments.BillingAddress_2, payments.BillingCty, payments.BillingState, payments.BillingZip);
                if (payments.PaymentMethod == "eCheck")
                {
                    var bankAccount = new AuthorizeNet.Api.Contracts.V1.bankAccountType
                    {
                        accountNumber = payments.BankAccountNumber,
                        routingNumber = payments.BankRoutingNumber,
                        echeckType = AuthorizeNet.Api.Contracts.V1.echeckTypeEnum.WEB,   // change based on how you take the payment (web, telephone, etc)
                        nameOnAccount = payments.FirstName + " " + payments.LastName
                    };
                    controller = PaymentGateway.DebitBankAccount(bankAccount, lineItems, billAddress, payments.OrderTotal, invoiceNumber);
                }
                else
                {
                    var creditCard = new AuthorizeNet.Api.Contracts.V1.creditCardType
                    {
                        cardNumber = payments.CardNo,
                        expirationDate = payments.CardExpMon + payments.CardExpYear.ToString(),
                        cardCode = payments.CVV
                    };
                    controller = PaymentGateway.ChargeCreditCard(creditCard, lineItems, billAddress, payments.OrderTotal, invoiceNumber);
                }
                AuthorizeNet.Api.Contracts.V1.createTransactionResponse response = controller.GetApiResponse();

                if (response != null)
                {
                    if (response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
                    {
                        if (response.transactionResponse != null)
                        {
                            if (response.transactionResponse.errors == null)
                            {
                                try
                                {
                                    string rentid = string.Join(",", rentidsarry);
                                    Payment pay = BGBCFunctions.RentPayment(((BGBC.Core.CustomPrincipal)(User)).UserId, invoiceNumber,
                                        response.transactionResponse.accountNumber, response.transactionResponse.accountType, response.transactionResponse.transId,
                                        response.transactionResponse.messages[0].code, response.transactionResponse.messages[0].description, Request.UserHostAddress, payments.OrderTotal, address, payments.Comments, rentid);

                                    IRepository<RentPayment, int> rentPay = new RentPaymentRepository();
                                    for (int i = 0; i < payments.TenantRent.Count; i++)
                                    {
                                        if (payments.TenantRent[i].ID == 0)
                                        {
                                            rentPay.Add(new RentPayment { PropertyID = payments.TenantRent[i].PropertyID, Description = payments.TenantRent[i].Charge, Amount = payments.TenantRent[i].AmountDue, PaymentID = pay.PaymentID });
                                        }
                                    }

                                    if (payments.SaveCard)
                                    {
                                        UserCC ccinfo = _userCCRep.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                                        if (ccinfo == null) //There is no details in database
                                        {
                                            if (payments.PaymentMethod == "eCheck")
                                            {
                                                _userCCRep.Add(new UserCC { UserID = ((BGBC.Core.CustomPrincipal)(User)).UserId, PaymentType = 2, AccountType = payments.BankAccountType, RoutingNo = Cryptography.Encrypt(payments.BankRoutingNumber), AccountNo = Cryptography.Encrypt(payments.BankAccountNumber) });
                                            }
                                            else
                                            {
                                                _userCCRep.Add(new UserCC { UserID = ((BGBC.Core.CustomPrincipal)(User)).UserId, PaymentType = 1, CCNO = Cryptography.Encrypt(payments.CardNo), ExpMon = Cryptography.Encrypt(payments.CardExpMon), ExpYear = Cryptography.Encrypt(payments.CardExpYear) });
                                            }
                                        }
                                        else
                                        {
                                            if (payments.PaymentMethod == "eCheck")
                                            {
                                                ccinfo.CCNO = string.Empty; ccinfo.ExpMon = string.Empty; ccinfo.ExpYear = string.Empty;
                                                ccinfo.PaymentType = 2; ccinfo.AccountType = payments.BankAccountType;
                                                ccinfo.RoutingNo = Cryptography.Encrypt(payments.BankRoutingNumber); ccinfo.AccountNo = Cryptography.Encrypt(payments.BankAccountNumber);
                                            }
                                            else
                                            {
                                                ccinfo.CCNO = Cryptography.Encrypt(payments.CardNo); ccinfo.ExpMon = Cryptography.Encrypt(payments.CardExpMon);
                                                ccinfo.ExpYear = Cryptography.Encrypt(payments.CardExpYear);
                                                ccinfo.PaymentType = 1; ccinfo.AccountType = string.Empty;
                                                ccinfo.RoutingNo = string.Empty; ccinfo.AccountNo = string.Empty;
                                            }
                                            _userCCRep.Update(ccinfo);
                                        }
                                    }
                                    else
                                    {
                                        UserCC ccinfo = _userCCRep.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                                        if (ccinfo != null) _userCCRep.Remove(ccinfo);
                                    }

                                    TempData.Remove("data");
                                    return RedirectToAction("MyAccount", "Tenant");
                                }
                                catch (Exception ex) { ModelState.AddModelError("", "Transaction Error : " + ex.Message); }
                                System.Diagnostics.Trace.TraceInformation("Success, Auth Code : " + response.transactionResponse.authCode);
                            }
                            else
                            { ModelState.AddModelError("", "Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText); }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Trace.TraceInformation("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                        if (response.transactionResponse != null)
                        {
                            ModelState.AddModelError("", "Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText);
                        }
                        TempData["data"] = payments;
                    }
                }
                else
                {
                    TempData["data"] = payments;
                    ModelState.AddModelError("", "Transaction Error, unable to complete the transaction.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Transaction Error, unable to complete the transaction.");
            }
            PopulateDropDown();
            return View("TenantPayment", payments);
        }

        [HttpGet]
        [Authorize]

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _userRepository.Get(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            PopulateDropDown();
            TenantInfo tenantInfo = new TenantInfo();
            tenantInfo.UserID = user.UserID;
            tenantInfo.ProfileInfo = user.Profiles.Where(x => x.UserID == id).Single();
            Tenant tenant = user.Tenants.Where(x => x.UserID == id).Single();
            tenantInfo.TenantID = tenant.TenantID;
            if (tenant == null)
            {
                return HttpNotFound();
            }
            Property _property = _propertyRepo.Get((int)tenant.PropertyID);

            tenantInfo.FirstName = user.FirstName;
            tenantInfo.LastName = user.LastName;
            tenantInfo.Email = user.Email;
            tenantInfo.ConfirmEmail = user.Email;

            tenantInfo.RentAmount = tenant.RentAmount;
            tenantInfo.FinalDueDate = tenant.FinalDueDate;
            tenantInfo.Deposit = tenant.Deposit;
            tenantInfo.DepostDueDate = tenant.DepostDueDate;
            tenantInfo.PetDepositDue = (bool)tenant.PetDepositDue;
            tenantInfo.PetDeposit = tenant.PetDeposit;

            tenantInfo.PropertyID = _property.PropertyID;
            tenantInfo.Address = _property.Address;
            tenantInfo.Address2 = _property.Address2;
            tenantInfo.City = _property.City;
            tenantInfo.State = _property.State;
            tenantInfo.Zip = _property.Zip;
            return View(tenantInfo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]

        public ActionResult DeletePost(int id)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    User _user = _userRepository.Get((int)id);
                    _userRepository.Remove(_user);
                    TempData["SucessMessage"] = "Tenant Deleted Successfully.";

                    return RedirectToAction("PropertyTenants", "Owner", new { id = _user.Tenants.FirstOrDefault().PropertyID });

                }

            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            return View("MyProperties", "Owner");
        }

        [Authorize]
        public ActionResult Add(int? id)
        {
            PopulateDropDown();

            if (TempData["tenantdata"] == null)
            {
                TenantInfo tenant = new TenantInfo();
                tenant.ProfileInfo = new Model.Profile();
                tenant.PetDepositDue = false;
                tenant.ProfileInfo.BillingAddressSame = true;

                Property property = _propertyRepo.Get((int)id);
                if (property != null)
                {
                    tenant.PropertyID = property.PropertyID;
                    tenant.PropertyName = property.Name;
                    tenant.Address = property.Address;
                    tenant.Address2 = property.Address2;
                    tenant.City = property.City;
                    tenant.State = property.State;
                    tenant.Zip = property.Zip;
                    tenant.OwnerName = string.Format("{0} {1}", property.User.FirstName, property.User.LastName);
                }
                return View(tenant);
            }
            else { return View("Add", (TenantInfo)TempData["tenantdata"]); }

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Confirm(TenantInfo tenantInfo)
        {
            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            if (!tenantInfo.ProfileInfo.BillingAddressSame)
            {
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, tenantInfo.ProfileInfo.BillingAddress, true, "Billing Address", "ProfileInfo.BillingAddress");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, tenantInfo.ProfileInfo.BillingAddress_2, false, "Billing Address 2", "ProfileInfo.BillingAddress_2");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, tenantInfo.ProfileInfo.BillingCty, true, "Billing City", "ProfileInfo.BillingCty");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, tenantInfo.ProfileInfo.BillingState, true, "Billing State", "ProfileInfo.BillingState");
                BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, tenantInfo.ProfileInfo.BillingZip, true, "Billing Zip", "ProfileInfo.BillingZip");
            }

            if (tenantInfo.ProfileInfo.HomePhone == null) ModelState.AddModelError("ProfileInfo.HomePhone", "The Home Phone field is required.");
            if (tenantInfo.FinalDueDate == null) ModelState.AddModelError("FinalDueDate", "The Final Due Date field is required.");
            if (tenantInfo.PetDepositDue) { if (string.IsNullOrEmpty(tenantInfo.PetDeposit.ToString())) ModelState.AddModelError("PetDeposit", "The Pet Deposit field is required."); }

            PopulateDropDown();
            ModelState["PropertyID"].Errors.Clear();
            if (ModelState.IsValid)
            {
                if (tenantInfo.ProfileInfo.BillingAddressSame)
                {
                    Property property = _propertyRepo.Get(tenantInfo.PropertyID);
                    tenantInfo.ProfileInfo.BillingAddress = property.Address;
                    tenantInfo.ProfileInfo.BillingAddress_2 = property.Address2;
                    tenantInfo.ProfileInfo.BillingCty = property.City;
                    tenantInfo.ProfileInfo.BillingState = property.State;
                    tenantInfo.ProfileInfo.BillingZip = property.Zip;
                }
                User user = _userRepository.Find(tenantInfo.Email);
                if (user != null)
                {
                    ModelState.AddModelError("Email", "Email is already exists");
                }
                else
                {
                    TempData["tenantdata"] = tenantInfo;
                    return View(tenantInfo);
                }
            }
            return View("Add", tenantInfo);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]

        public ActionResult Add(TenantInfo tenantInfo)
        {
            try
            {

                PopulateDropDown();
                tenantInfo = (TenantInfo)TempData["tenantdata"];
                User user = new User();
                user.FirstName = tenantInfo.FirstName;
                user.LastName = tenantInfo.LastName;
                user.Email = tenantInfo.Email;
                user.Password = BGBC.Core.Security.Cryptography.Encrypt(BGBC.Core.Security.Cryptography.RandomString(8));
                user.UserType = 2;

                user.Profiles.Add(tenantInfo.ProfileInfo);
                //Profile profile = tenantInfo.ProfileInfo;
                //profile.UserID = user.UserID;
                ////_profileRepo.Add(profile);

                Tenant tenant = new Tenant();
                tenant.PropertyID = tenantInfo.PropertyID;

                tenant.RentAmount = tenantInfo.RentAmount;
                tenant.FinalDueDate = tenantInfo.FinalDueDate;
                tenant.Deposit = tenantInfo.Deposit;
                tenant.DepostDueDate = tenantInfo.DepostDueDate;
                tenant.PetDepositDue = tenantInfo.PetDepositDue;
                tenant.PetDeposit = tenantInfo.PetDeposit;
                user.Tenants.Add(tenant);
                //_tenantRepo.Add(tenant);
                //user.Profiles.Add(profile);
                //user.Tenants.Add(tenant);
                //user.Tenants.Add(tenant);
                //_userRepository.Add(user);
                user.UserReferences.Add(new UserReference());
                user.UserReferences.Add(new UserReference());
                user = _userRepository.Add(user);
                TempData["SucessMessage"] = "Tenant " + tenantInfo.FirstName + " " + tenantInfo.LastName + " Added  successfully.";
                var token = BGBC.Core.Security.Cryptography.RandomString(32);
                PasswordReset passwordreset = new PasswordReset();
                passwordreset.EmailID = user.Email;
                passwordreset.Token = token;
                passwordresetRepo.Add(passwordreset);
                BGBC.Web.Utilities.MailUtility obj = new Utilities.MailUtility();
                obj.TenantAdd(user.Email, user.FirstName + " " + user.LastName, "http://" + HttpContext.Request.Url.Authority + "/Home/ResetPassword/" + token);
                return RedirectToAction("PropertyTenants", "Owner", new { id = tenantInfo.PropertyID });
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDropDown();
            return View(tenantInfo);
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _userRepository.Get((int)id);
            if (user == null)
            {
                return HttpNotFound();
            }
            PopulateDropDown();
            if (TempData["tenantdata"] == null)
            {

                TenantInfo tenantInfo = new TenantInfo();
                tenantInfo.UserID = user.UserID;
                tenantInfo.ProfileInfo = user.Profiles.Where(x => x.UserID == id).SingleOrDefault();
                Tenant tenant = user.Tenants.Where(x => x.UserID == id).SingleOrDefault();
                tenantInfo.TenantID = user.UserID;
                if (tenant == null)
                {
                    return HttpNotFound();
                }
                Property _property = _propertyRepo.Get((int)tenant.PropertyID);
                tenantInfo.PropertyName = _property.Name;

                tenantInfo.FirstName = user.FirstName;
                tenantInfo.LastName = user.LastName;
                tenantInfo.Email = user.Email;
                tenantInfo.ConfirmEmail = user.Email;

                tenantInfo.RentAmount = tenant.RentAmount;
                tenantInfo.FinalDueDate = tenant.FinalDueDate;
                tenantInfo.Deposit = tenant.Deposit;
                tenantInfo.DepostDueDate = tenant.DepostDueDate;
                tenantInfo.PetDepositDue = (bool)tenant.PetDepositDue;
                tenantInfo.PetDeposit = tenant.PetDeposit;

                tenantInfo.PropertyID = (int)tenant.PropertyID;
                tenantInfo.Address = _property.Address;
                tenantInfo.Address2 = _property.Address2;
                tenantInfo.City = _property.City;
                tenantInfo.State = _property.State;
                tenantInfo.Zip = _property.Zip;
                tenantInfo.OwnerName = string.Format("{0} {1}", _property.User.FirstName, _property.User.LastName);

                ViewBag.Url = (Request.UrlReferrer != null && Request.UrlReferrer.Segments.Length > 2 ? Request.UrlReferrer.Segments[2].ToString().Trim('/') : "");

                return View(tenantInfo);
            }
            else { return View("Edit", (TenantInfo)TempData["tenantdata"]); }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmEdit(TenantInfo tenantInfo)
        {
            var query = from state in ModelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            if (!tenantInfo.ProfileInfo.BillingAddressSame)
            {
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, tenantInfo.ProfileInfo.BillingAddress, true, "Billing Address", "ProfileInfo.BillingAddress");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, tenantInfo.ProfileInfo.BillingAddress_2, false, "Billing Address 2", "ProfileInfo.BillingAddress_2");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, tenantInfo.ProfileInfo.BillingCty, true, "Billing City", "ProfileInfo.BillingCty");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, tenantInfo.ProfileInfo.BillingState, true, "Billing State", "ProfileInfo.BillingState");
                BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, tenantInfo.ProfileInfo.BillingZip, true, "Billing Zip", "ProfileInfo.BillingZip");
            }
            PopulateDropDown();
            ModelState["Zip"].Errors.Clear();
            ModelState["PropertyID"].Errors.Clear();
            if (ModelState.IsValid)
            {
                //Address is same as Proprty address
                if (tenantInfo.ProfileInfo.BillingAddressSame)
                {
                    tenantInfo.ProfileInfo.BillingAddress = tenantInfo.Address;
                    tenantInfo.ProfileInfo.BillingAddress_2 = tenantInfo.Address2;
                    tenantInfo.ProfileInfo.BillingCty = tenantInfo.City;
                    tenantInfo.ProfileInfo.BillingState = tenantInfo.State;
                    tenantInfo.ProfileInfo.BillingZip = tenantInfo.Zip;
                }

                TempData["tenantdata"] = tenantInfo;
                return View(tenantInfo);

            }
            return View("Edit", tenantInfo);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize]

        public ActionResult EditPost(TenantInfo tenantInfo)
        {
            if (tenantInfo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                PopulateDropDown();
                tenantInfo = (TenantInfo)TempData["tenantdata"];
                //ModelState["AltEmail"].Errors.Clear();
                User user = _userRepository.Get((int)tenantInfo.UserID);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.FirstName = tenantInfo.FirstName;
                user.LastName = tenantInfo.LastName;
                user.Email = tenantInfo.Email;
                _userRepository.Update(user);

                Profile profile = _profileRepo.Get(tenantInfo.ProfileInfo.ProfileID);
                profile.BillingAddressSame = tenantInfo.ProfileInfo.BillingAddressSame;
                profile.BillingAddress = tenantInfo.ProfileInfo.BillingAddress;
                profile.BillingAddress_2 = tenantInfo.ProfileInfo.BillingAddress_2;
                profile.BillingCty = tenantInfo.ProfileInfo.BillingCty;
                profile.BillingState = tenantInfo.ProfileInfo.BillingState;
                profile.BillingZip = tenantInfo.ProfileInfo.BillingZip;
                profile.HomePhone = tenantInfo.ProfileInfo.HomePhone;
                profile.WorkPhone = tenantInfo.ProfileInfo.WorkPhone;
                profile.MobilePhone = tenantInfo.ProfileInfo.MobilePhone;
                _profileRepo.Update(profile);
                TempData["SucessMessage"] = "Tenant updated successfully.";
                return RedirectToAction("PropertyTenants", "Owner", new { id = tenantInfo.PropertyID });

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDropDown();
            return View(tenantInfo);
        }

        [Authorize]
        [CustomAuthorize(Roles = "Admin")]
        public ActionResult EditAdmin(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = _userRepository.Get(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            PopulateDropDown();
            if (TempData["tenantdata"] == null)
            {
                TenantInfo tenantInfo = new TenantInfo();
                tenantInfo.UserID = user.UserID;
                tenantInfo.ProfileInfo = user.Profiles.Where(x => x.UserID == id).Single();
                Tenant tenant = user.Tenants.Where(x => x.UserID == id).Single();
                tenantInfo.TenantID = tenant.TenantID;
                if (tenant == null)
                {
                    return HttpNotFound();
                }
                Property _property = _propertyRepo.Get((int)tenant.PropertyID);

                tenantInfo.FirstName = user.FirstName;
                tenantInfo.LastName = user.LastName;
                tenantInfo.Email = user.Email;
                tenantInfo.ConfirmEmail = user.Email;

                tenantInfo.RentAmount = tenant.RentAmount;
                tenantInfo.FinalDueDate = tenant.FinalDueDate;
                tenantInfo.Deposit = tenant.Deposit;
                tenantInfo.DepostDueDate = tenant.DepostDueDate;
                tenantInfo.PetDepositDue = (bool)tenant.PetDepositDue;
                tenantInfo.PetDeposit = tenant.PetDeposit;

                tenantInfo.PropertyID = _property.PropertyID;
                tenantInfo.Address = _property.Address;
                tenantInfo.Address2 = _property.Address2;
                tenantInfo.City = _property.City;
                tenantInfo.State = _property.State;
                tenantInfo.Zip = _property.Zip;
                ViewBag.UserID = _property.UserID;
                ViewBag.PropertyID = _property.PropertyID;

               
                tenantInfo.PropertyName = _property.Name;
                tenantInfo.OwnerName = string.Format("{0} {1}", _property.User.FirstName, _property.User.LastName);

                // Get the action name of Url
                ViewBag.Url = Request.UrlReferrer.Segments[2].ToString().Trim('/');

                return View(tenantInfo);
            }
            else
            {
                Tenant tenant = user.Tenants.Where(x => x.UserID == id).Single();
                Property _property = _propertyRepo.Get((int)tenant.PropertyID);
                ViewBag.UserID = _property.UserID;
                ViewBag.PropertyID = _property.PropertyID;
                return View("EditAdmin", (TenantInfo)TempData["tenantdata"]);
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmEditAdmin(TenantInfo tenantInfo)
        {
            if (!tenantInfo.ProfileInfo.BillingAddressSame)
            {
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, tenantInfo.ProfileInfo.BillingAddress, true, "Billing Address", "ProfileInfo.BillingAddress");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, tenantInfo.ProfileInfo.BillingAddress_2, false, "Billing Address 2", "ProfileInfo.BillingAddress_2");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, tenantInfo.ProfileInfo.BillingCty, true, "Billing City", "ProfileInfo.BillingCty");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, tenantInfo.ProfileInfo.BillingState, true, "Billing State", "ProfileInfo.BillingState");
                BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, tenantInfo.ProfileInfo.BillingZip, true, "Billing Zip", "ProfileInfo.BillingZip");
            }
            if (tenantInfo.PetDepositDue) { if (string.IsNullOrEmpty(tenantInfo.PetDeposit.ToString())) ModelState.AddModelError("PetDeposit", "The Pet Deposit field is required."); }
            Property _property = _propertyRepo.Get((int)tenantInfo.PropertyID);
            ViewBag.UserID = _property.UserID;
            PopulateDropDown();
            ModelState["PropertyID"].Errors.Clear();
            if (ModelState.IsValid)
            {
                if (tenantInfo.ProfileInfo.BillingAddressSame)
                {
                    tenantInfo.ProfileInfo.BillingAddress = tenantInfo.Address;
                    tenantInfo.ProfileInfo.BillingAddress_2 = tenantInfo.Address2;
                    tenantInfo.ProfileInfo.BillingCty = tenantInfo.City;
                    tenantInfo.ProfileInfo.BillingState = tenantInfo.State;
                    tenantInfo.ProfileInfo.BillingZip = tenantInfo.Zip;
                }
                TempData["tenantdata"] = tenantInfo;
                return View(tenantInfo);

            }
            return View("EditAdmin", tenantInfo);
        }

        [HttpPost, ActionName("EditAdmin")]
        [ValidateAntiForgeryToken]
        [Authorize]
        [CustomAuthorize(Roles = "Admin")]

        public ActionResult EditAdmin(TenantInfo tenantInfo)
        {
            if (tenantInfo == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                PopulateDropDown();
                tenantInfo = (TenantInfo)TempData["tenantdata"];
                User user = _userRepository.Get((int)tenantInfo.UserID);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.FirstName = tenantInfo.FirstName;
                user.LastName = tenantInfo.LastName;
                user.Email = tenantInfo.Email;
                _userRepository.Update(user);

                Profile profile = _profileRepo.Get(tenantInfo.ProfileInfo.ProfileID);
                profile.BillingAddressSame = tenantInfo.ProfileInfo.BillingAddressSame;
                profile.BillingAddress = tenantInfo.ProfileInfo.BillingAddress;
                profile.BillingAddress_2 = tenantInfo.ProfileInfo.BillingAddress_2;
                profile.BillingCty = tenantInfo.ProfileInfo.BillingCty;
                profile.BillingState = tenantInfo.ProfileInfo.BillingState;
                profile.BillingZip = tenantInfo.ProfileInfo.BillingZip;
                profile.HomePhone = tenantInfo.ProfileInfo.HomePhone;
                profile.WorkPhone = tenantInfo.ProfileInfo.WorkPhone;
                profile.MobilePhone = tenantInfo.ProfileInfo.MobilePhone;
                _profileRepo.Update(profile);

                Tenant tenant = _tenantRepo.Get(tenantInfo.TenantID);

                tenant.RentAmount = tenantInfo.RentAmount;
                tenant.FinalDueDate = tenantInfo.FinalDueDate;
                tenant.Deposit = tenantInfo.Deposit;
                tenant.DepostDueDate = tenantInfo.DepostDueDate;
                tenant.PetDepositDue = tenantInfo.PetDepositDue;
                tenant.PetDeposit = tenantInfo.PetDeposit;
                _tenantRepo.Update(tenant);
                TempData["SucessMessage"] = "Tenant Updated successfully.";
                return RedirectToAction("PropertyTenants", "Admin", new { id = tenantInfo.PropertyID });

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDropDown();
            return View(tenantInfo);
        }

        [Authorize]
        public ActionResult AddTenant()
        {
            return View();
        }

        [Authorize]
        public ActionResult EditTenant()
        {
            PopulateDropDown();
            return View();
        }

        [CustomAuthorize(Roles = "Tenant")]
        [Authorize]
        public ActionResult Profile()
        {
            BGBC.Model.User user = _userRepository.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            Profile profile = _profileRepo.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
            List<UserReference> _ref = _userRefRepo.GetRef(((BGBC.Core.CustomPrincipal)(User)).UserId);
            TenantProfile tenantprofile;
            if (_ref == null)
            {
                tenantprofile = new TenantProfile { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, ConfirmEmail = user.Email, Createdon = user.Createdon, Updatedon = user.Updatedon, AltEmail = profile.AltEmail, ConfirmAltEmail = profile.AltEmail, ProfileInfo = profile, Ref1 = new UserReference(), Ref2 = new UserReference() };
            }
            else
            {
                tenantprofile = new TenantProfile { FirstName = user.FirstName, LastName = user.LastName, Email = user.Email, ConfirmEmail = user.Email, Createdon = user.Createdon, Updatedon = user.Updatedon, AltEmail = profile.AltEmail, ConfirmAltEmail = profile.AltEmail, ProfileInfo = profile, Ref1 = (_ref.Count > 0 ? _ref[0] : new UserReference()), Ref2 = (_ref.Count > 1 ? _ref[1] : new UserReference()) };
            }

            RentAutoPay rentpay = _rentAutoPayRep.Get(user.UserID);
            if (rentpay == null)
            {
                tenantprofile.PaymentMethod = "eCheck";
                Tenant tent = user.Tenants.FirstOrDefault();
                if (tent != null)
                {
                    tenantprofile.MonthlyRent = (decimal)tent.RentAmount;
                    tenantprofile.ServiceFee = Math.Round(((decimal)tent.RentAmount * (10.75m / 100.00m)), 2);
                    tenantprofile.TotalCharges = tenantprofile.MonthlyRent + tenantprofile.ServiceFee;
                }

                UserCC ccinfo = _userCCRep.Get(((BGBC.Core.CustomPrincipal)(User)).UserId);
                if (ccinfo != null)
                {
                    if (ccinfo.PaymentType == 1)
                    {
                        tenantprofile.PaymentMethod = "Credit Card"; tenantprofile.CardNo = Cryptography.Decrypt(ccinfo.CCNO); tenantprofile.CardExpMon = Cryptography.Decrypt(ccinfo.ExpMon);
                        tenantprofile.CardExpYear = Cryptography.Decrypt(ccinfo.ExpYear);
                    }
                    else { tenantprofile.PaymentMethod = "eCheck"; tenantprofile.BankRoutingNumber = Cryptography.Decrypt(ccinfo.RoutingNo); tenantprofile.BankAccountNumber = Cryptography.Decrypt(ccinfo.AccountNo); tenantprofile.BankAccountType = ccinfo.AccountType; }
                }
            }
            else
            {
                tenantprofile.ChargeAccount = true;
                tenantprofile.MonthlyRent = rentpay.Rent;
                tenantprofile.ServiceFee = rentpay.Charges;
                tenantprofile.TotalCharges = rentpay.TotalAmt;

                if (rentpay.PaymentType == 1)
                {
                    tenantprofile.PaymentMethod = "Credit Card"; tenantprofile.CardNo = Cryptography.Decrypt(rentpay.CCNO); tenantprofile.CardExpMon = Cryptography.Decrypt(rentpay.ExpMon);
                    tenantprofile.CardExpYear = Cryptography.Decrypt(rentpay.ExpYear); tenantprofile.CVV = Cryptography.Decrypt(rentpay.CVV);
                }
                else { tenantprofile.PaymentMethod = "eCheck"; tenantprofile.BankRoutingNumber = Cryptography.Decrypt(rentpay.RoutingNo); tenantprofile.BankAccountNumber = Cryptography.Decrypt(rentpay.AccountNo); tenantprofile.BankAccountType = rentpay.AccountType; }

            }
            PopulateDropDown();
            return View(tenantprofile);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Roles = "Tenant")]
        public ActionResult Profile(TenantProfile userprofile)
        {
            try
            {
                //Adding Regularexpression

                if (!userprofile.ProfileInfo.BillingAddressSame)
                {
                    BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.ProfileInfo.BillingAddress, true, "Address", "ProfileInfo.BillingAddress");
                    BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.ProfileInfo.BillingAddress_2, false, "Address 2", "ProfileInfo.BillingAddress_2");
                    BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.ProfileInfo.BillingCty, true, "City", "ProfileInfo.BillingCty");
                    BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.ProfileInfo.BillingState, true, "State", "ProfileInfo.BillingState");
                    BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, userprofile.ProfileInfo.BillingZip, true, "Zip", "ProfileInfo.BillingZip");
                }

                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref1.Name, true, "Address", "Ref1.Name");
                if (string.IsNullOrEmpty(userprofile.Ref1.Phone)) ModelState.AddModelError("Ref1.Phone", "The Phone field is required");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.Ref1.EMail, true, "Email", "Ref1.EMail");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.Ref1.Address, true, "Address", "Ref1.Address");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.Ref1.Address2, false, "Address 2", "Ref1.Address2");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref1.City, true, "City", "Ref1.City");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref1.State, true, "State", "Ref1.State");
                BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, userprofile.Ref1.Zip, true, "Zip", "Ref1.Zip");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref1.Relationship, true, "Relationship", "Ref1.Relationship");

                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref2.Name, true, "Address", "Ref2.Name");
                if (string.IsNullOrEmpty(userprofile.Ref2.Phone)) ModelState.AddModelError("Ref2.Phone", "The Phone field is required");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.Ref2.EMail, true, "Email", "Ref2.EMail");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.Ref2.Address, true, "Address", "Ref2.Address");
                BGBC.Core.ModelDataValidation.Instance.AlphaNumeric(ModelState, userprofile.Ref2.Address2, false, "Address 2", "Ref2.Address2");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref2.City, true, "City", "Ref2.City");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref2.State, true, "State", "Ref2.State");
                BGBC.Core.ModelDataValidation.Instance.Zip(ModelState, userprofile.Ref2.Zip, true, "Zip", "Ref2.Zip");
                BGBC.Core.ModelDataValidation.Instance.Alpha(ModelState, userprofile.Ref2.Relationship, true, "Relationship", "Ref2.Relationship");

                if ((!string.IsNullOrEmpty(userprofile.NewPassword)) && (string.IsNullOrEmpty(userprofile.CurrentPassword)))
                {
                    ModelState.AddModelError("CurrentPassword", "The Current Password field is required.");
                }

                if (userprofile.ChargeAccount)
                {
                    if (userprofile.PaymentMethod == "eCheck")
                    {
                        if (string.IsNullOrEmpty(userprofile.BankAccountType)) ModelState.AddModelError("BankAccountType", "Please select Bank Account Type");
                        if (!string.IsNullOrEmpty(userprofile.BankRoutingNumber))
                        {
                            if (userprofile.BankRoutingNumber.Trim().Length != 9)
                                ModelState.AddModelError("BankRoutingNumber", "Please enter a valid routing number");
                        }
                        else
                        {
                            ModelState.AddModelError("BankRoutingNumber", "Please enter a valid routing number");
                        }
                        if (!string.IsNullOrEmpty(userprofile.BankAccountNumber))
                        {
                            if (userprofile.BankAccountNumber.Trim().Length != 7)
                                ModelState.AddModelError("BankAccountNumber", "Please enter a valid account number");
                        }
                        else
                        {
                            ModelState.AddModelError("BankAccountNumber", "Please enter a valid account number");
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(userprofile.CardNo)) ModelState.AddModelError("CardNo", "The Card No field is required.");
                        if (string.IsNullOrEmpty(userprofile.CVV)) ModelState.AddModelError("CVV", "The Card CVV field is required.");
                        if (!string.IsNullOrEmpty(userprofile.CardNo))
                        {
                            if (userprofile.CardNo.Trim().Length > 0)
                            {
                                if (CreditCardUtility.IsValidNumber(userprofile.CardNo))
                                {
                                    if (!string.IsNullOrEmpty(userprofile.CVV))
                                    {
                                        CreditCardTypeType? cardType = CreditCardUtility.GetCardTypeFromNumber(userprofile.CardNo);
                                        if (cardType == null)
                                        {
                                            ModelState.AddModelError("CardNo", "Please enter a valid card number");
                                        }
                                        else
                                        {
                                            userprofile.CardType = (CreditCardTypeType)cardType;
                                            if (cardType == CreditCardTypeType.Amex && userprofile.CVV.Trim().Length != 4) ModelState.AddModelError("CVV", "Please enter a valid CVV number");
                                            else if (cardType != CreditCardTypeType.Amex && userprofile.CVV.Trim().Length != 3) ModelState.AddModelError("CVV", "Please enter a valid CVV number");
                                        }
                                    }
                                }
                                else
                                {
                                    ModelState.AddModelError("CardNo", "Please enter a valid card number");
                                }
                            }
                        }
                    }
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
                            PopulateDropDown();
                            ModelState.AddModelError("CurrentPassword", "The CurrentPassword field is required.");
                            return View(userprofile);
                        }
                    }

                    selUser.FirstName = userprofile.FirstName;
                    selUser.LastName = userprofile.LastName;
                    selUser.Email = userprofile.Email;
                    userprofile.ProfileInfo.UserID = selUser.UserID;
                    userprofile.ProfileInfo.AltEmail = userprofile.AltEmail;
                    userprofile.Ref1.UserID = selUser.UserID;
                    userprofile.Ref2.UserID = selUser.UserID;

                    _userRepository.Update(selUser);
                    _profileRepo.Update(userprofile.ProfileInfo);
                    if (userprofile.Ref1.UserReferenceID == 0) _userRefRepo.Add(userprofile.Ref1); else _userRefRepo.Update(userprofile.Ref1);
                    if (userprofile.Ref2.UserReferenceID == 0) _userRefRepo.Add(userprofile.Ref2); else _userRefRepo.Update(userprofile.Ref2);


                    if (userprofile.ChargeAccount)
                    {
                        RentAutoPay rentpay = _rentAutoPayRep.Get(selUser.UserID);
                        if (rentpay == null)
                        {
                            if (userprofile.PaymentMethod == "eCheck")
                            {
                                _rentAutoPayRep.Add(new RentAutoPay { UserID = selUser.UserID, PaymentType = 2, AccountType = userprofile.BankAccountType, RoutingNo = Cryptography.Encrypt(userprofile.BankRoutingNumber), AccountNo = Cryptography.Encrypt(userprofile.BankAccountNumber), Rent = userprofile.MonthlyRent, Charges = userprofile.ServiceFee, TotalAmt = userprofile.TotalCharges });
                            }
                            else
                            {
                                _rentAutoPayRep.Add(new RentAutoPay { UserID = selUser.UserID, PaymentType = 1, CCNO = Cryptography.Encrypt(userprofile.CardNo), ExpMon = Cryptography.Encrypt(userprofile.CardExpMon), ExpYear = Cryptography.Encrypt(userprofile.CardExpYear), CVV = Cryptography.Encrypt(userprofile.CVV), Rent = userprofile.MonthlyRent, Charges = userprofile.ServiceFee, TotalAmt = userprofile.TotalCharges });
                            }
                        }
                        else
                        {
                            if (userprofile.PaymentMethod == "eCheck")
                            {
                                rentpay.CCNO = string.Empty; rentpay.ExpMon = string.Empty; rentpay.ExpYear = string.Empty;
                                rentpay.PaymentType = 2; rentpay.AccountType = userprofile.BankAccountType;
                                rentpay.RoutingNo = Cryptography.Encrypt(userprofile.BankRoutingNumber); rentpay.AccountNo = Cryptography.Encrypt(userprofile.BankAccountNumber);
                            }
                            else
                            {
                                rentpay.CCNO = Cryptography.Encrypt(userprofile.CardNo); rentpay.ExpMon = Cryptography.Encrypt(userprofile.CardExpMon);
                                rentpay.ExpYear = Cryptography.Encrypt(userprofile.CardExpYear); rentpay.CVV = Cryptography.Encrypt(userprofile.CVV);
                                rentpay.PaymentType = 1; rentpay.AccountType = string.Empty;
                                rentpay.RoutingNo = string.Empty; rentpay.AccountNo = string.Empty;
                            }
                            rentpay.Rent = userprofile.MonthlyRent;
                            rentpay.Charges = userprofile.ServiceFee;
                            rentpay.TotalAmt = userprofile.TotalCharges;
                        }
                    }
                    else
                    {
                        RentAutoPay rentpay = _rentAutoPayRep.Get(selUser.UserID);
                        if (rentpay != null) _rentAutoPayRep.Remove(rentpay);
                    }
                    ViewBag.SucessMessage = "Profile is updated successfull";
                    PopulateDropDown();
                    return View();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateDropDown();
            return View(userprofile);
        }

        [Authorize]
        public ActionResult ViewAllTenantPayments(int id)
        {
            List<BGBC.Web.Models.AllPropertiesAndTenant> allPropertiesAndTenant = new List<Models.AllPropertiesAndTenant>();

            try
            {
                Property property = _propertyRepo.Get(id);
                ViewBag.UserId = property.UserID;
                ViewBag.PropertyID = property.PropertyID;
                BGBC.Web.Models.AllPropertiesAndTenant pp = new Models.AllPropertiesAndTenant();
                pp.Pname = property.Name;
                pp.Address = property.Address;
                pp.Address2 = property.Address2;
                pp.state = property.State;
                pp.Zip = property.Zip;
                pp.tenantRentPay = new List<Models.TenantRentPay>();
                foreach (var t in property.Tenants)
                {
                    BGBC.Web.Models.TenantRentPay ttttttttt = new BGBC.Web.Models.TenantRentPay();
                    ttttttttt.tname = t.User.FirstName;
                    ttttttttt.RentPayment = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == t.User.UserID).Take(5).ToList();
                    pp.tenantRentPay.Add(ttttttttt);
                }
                allPropertiesAndTenant.Add(pp);
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
            }

            return View(allPropertiesAndTenant);
        }

        [Authorize]
        public ActionResult ViewTenantsPayments(int id)
        {
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
                log.Error(ex.Message);
            }
            return View(new List<vRentPayment>());
        }

        [Authorize]

        public ActionResult ViewPayments(int Id)
        {
            User tuser = _userRepository.Get(Id);
            ViewBag.Name = tuser.FirstName + " " + tuser.LastName;
            List<BGBC.Web.Models.AllPropertiesAndTenant> allPropertiesAndTenant = new List<Models.AllPropertiesAndTenant>();
            List<Property> property = _propertyRepo.GetRef(Id);
            foreach (var p in property)
            {
                BGBC.Web.Models.AllPropertiesAndTenant pp = new Models.AllPropertiesAndTenant();
                pp.Pname = p.Name;
                pp.tenantRentPay = new List<Models.TenantRentPay>();
                foreach (var t in p.Tenants)
                {
                    BGBC.Web.Models.TenantRentPay ttttttttt = new BGBC.Web.Models.TenantRentPay();
                    ttttttttt.tuserid = t.UserID;
                    ttttttttt.tname = t.User.FirstName + " " + t.User.LastName;
                    ttttttttt.RentPayment = BGBCFunctions.RentPayments().Where(x => x.TenantUserID == t.User.UserID).Take(5).OrderByDescending(o => o.TransDate).ToList();
                    pp.tenantRentPay.Add(ttttttttt);
                }
                allPropertiesAndTenant.Add(pp);
            }

            return View(allPropertiesAndTenant);


        }

        private void PopulateDropDown()
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