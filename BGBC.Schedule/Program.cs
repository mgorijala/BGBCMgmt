using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BGBC.Model;
using BGBC.Core;

namespace BGBC.Schedule
{
    class Program
    {
        static void Main(string[] args)
        {
            //Call Rent calculation SP

            try
            {
                BGBC.Model.BGBCFunctions.RentCalcSchedule(DateTime.Today.Date);
            }
            catch (Exception ex)
            {


            }

            IRepository<RentAutoPay, int> rentAutoRepo = new RentAutoPayRepository();
            IRepository<RentDue, int?> rentDueRepo = new RentDueRepository();
            try
            {
                IEnumerable<RentAutoPay> listRentPay = listRentPay = rentAutoRepo.Get();
                foreach (var rentPayitem in listRentPay)
                {
                    RentDue rentDue = rentDueRepo.Get().Where(x => x.UserID == rentPayitem.UserID && x.DueStatus == true).FirstOrDefault();
                    if (rentDue != null)
                    {
                        try
                        {
                            User user = rentDue.User;
                            Profile profile = rentDue.User.Profiles.FirstOrDefault();
                            var lineItems = new AuthorizeNet.Api.Contracts.V1.lineItemType[2];
                            int[] rentidsarry = new int[2];
                            string fee = "Service Fee(10.75%)";
                            lineItems[0] = new AuthorizeNet.Api.Contracts.V1.lineItemType { itemId = "1", name = (rentDue.Description.Length < 30) ? rentDue.Description : rentDue.Description.Substring(0, 30), quantity = 1, unitPrice = rentDue.DueAmount };
                            lineItems[1] = new AuthorizeNet.Api.Contracts.V1.lineItemType { itemId = "1", name = (fee.Length < 30) ? fee : fee.Substring(0, 30), quantity = 1, unitPrice = rentPayitem.Charges };

                            int invoiceNumber = BGBCFunctions.GetInoiveNo();
                            AuthorizeNet.Api.Controllers.createTransactionController controller;
                            var billAddress = new AuthorizeNet.Api.Contracts.V1.customerAddressType { firstName = user.FirstName, lastName = user.LastName, address = profile.BillingAddress + ", " + (string.IsNullOrEmpty(profile.BillingAddress_2) ? "" : profile.BillingAddress_2), city = profile.BillingCty, state = profile.BillingState, zip = profile.BillingZip, email = user.Email, phoneNumber = profile.MobilePhone, country = "USA" };
                            string address = string.Format("{0}<br/>{1}<br/>{2}, {3} {4}", profile.BillingAddress, profile.BillingAddress_2, profile.BillingCty, profile.BillingState, profile.BillingZip);
                            if (rentPayitem.PaymentType == 2)
                            {
                                var bankAccount = new AuthorizeNet.Api.Contracts.V1.bankAccountType
                                {
                                    accountNumber = rentPayitem.AccountNo,
                                    routingNumber = rentPayitem.RoutingNo,
                                    echeckType = AuthorizeNet.Api.Contracts.V1.echeckTypeEnum.WEB,   // change based on how you take the payment (web, telephone, etc)
                                    nameOnAccount = user.FirstName + " " + user.LastName
                                };
                                controller = PaymentGateway.DebitBankAccount(bankAccount, lineItems, billAddress, rentPayitem.TotalAmt, invoiceNumber);
                            }
                            else
                            {
                                var creditCard = new AuthorizeNet.Api.Contracts.V1.creditCardType
                                {
                                    cardNumber = rentPayitem.CCNO,
                                    expirationDate = rentPayitem.ExpMon + rentPayitem.ExpYear.ToString(),
                                    cardCode = rentPayitem.CVV
                                };
                                controller = PaymentGateway.ChargeCreditCard(creditCard, lineItems, billAddress, rentPayitem.TotalAmt, invoiceNumber);
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
                                                Payment pay = BGBCFunctions.RentPayment(user.UserID, invoiceNumber,
                                                    response.transactionResponse.accountNumber, response.transactionResponse.accountType, response.transactionResponse.transId,
                                                    response.transactionResponse.messages[0].code, response.transactionResponse.messages[0].description, "", rentPayitem.TotalAmt, address, "Auto rent pay", rentDue.RentDueID.ToString());

                                                IRepository<RentPayment, int> rentPay = new RentPaymentRepository();

                                                rentPay.Add(new RentPayment { PropertyID = rentDue.PropertyID, Description = fee, Amount = rentPayitem.Charges, PaymentID = pay.PaymentID });

                                            }
                                            catch (Exception ex) { Console.WriteLine("Transaction Error : " + ex.Message); }
                                            System.Diagnostics.Trace.TraceInformation("Success, Auth Code : " + response.transactionResponse.authCode);
                                        }
                                        else
                                        { Console.WriteLine("Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText); }
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Trace.TraceInformation("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                                    if (response.transactionResponse != null)
                                    {
                                        Console.WriteLine("Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Transaction Error, unable to complete the transaction.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("Transaction Error, unable to complete the transaction.");
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}