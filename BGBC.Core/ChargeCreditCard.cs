using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Core
{
    public static class PaymentGateway
    {
        static string ApiLoginID = "4P58Pgv3R8cB";
        static string ApiTransactionKey = "6gcQX9HcaZ593v2V";

        public static createTransactionController ChargeCreditCard(creditCardType creditCard, lineItemType[] lineItems, customerAddressType address, decimal billamount, int invoiceNumber)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey,
            };

            //var creditCard = new creditCardType
            //{
            //    cardNumber = "4111111111111111",
            //    expirationDate = "0718",
            //    cardCode = "123"
            //};

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
                amount = billamount,
                payment = paymentType,
                lineItems = lineItems,
                billTo = address
            };
            var order = new AuthorizeNet.Api.Contracts.V1.orderType { invoiceNumber = "INV-" + invoiceNumber.ToString(), description = "Product Purchases" };

            transactionRequest.order = order;
            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            return controller;
        }

        public static createTransactionController DebitBankAccount(bankAccountType bankAccount, lineItemType[] lineItems, customerAddressType address, decimal billamount, int invoiceNumber)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };

            //var bankAccount = new bankAccountType
            //{
            //    accountNumber = "4111111",
            //    routingNumber = "325070760",
            //    echeckType = echeckTypeEnum.WEB,   // change based on how you take the payment (web, telephone, etc)
            //    nameOnAccount = "Test Name"
            //};

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = bankAccount };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
                amount = billamount,
                payment = paymentType,
                lineItems = lineItems,
                billTo = address
            };
            var order = new AuthorizeNet.Api.Contracts.V1.orderType { invoiceNumber = "INV-" + invoiceNumber.ToString(), description = "Product Purchases" };
            transactionRequest.order = order;
            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            return controller;
        }
    }
}
