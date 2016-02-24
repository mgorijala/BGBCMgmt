using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model
{
    public static class BGBCFunctions
    {
        static BGBCEntities context = new BGBCEntities();

        public static int GetInoiveNo()
        {
            return (int)context.SP_NextInvoiceNo().FirstOrDefault();
        }

        public static Order ProductTrans(int? UserID, string Email, string Password, Nullable<int> i_InvoiceID, string v_AccountNumber, string v_AccountType, string v_TransId, string v_StatusCode, string v_StatusText, string v_CustomerIP, string v_Address, string v_Comments, string v_Products)
        {
            return context.SP_ProductTrans(UserID, i_InvoiceID, v_AccountNumber, v_AccountType, v_TransId, v_StatusCode, v_StatusText, v_CustomerIP, v_Address, v_Comments, v_Products).FirstOrDefault();
        }

        public static Payment RentPayment(int? UserID, Nullable<int> i_InvoiceID, string v_AccountNumber, string v_AccountType, string v_TransId, string v_StatusCode, string v_StatusText, string v_CustomerIP, Nullable<decimal> i_Total, string v_Address, string v_Comments, string v_RentDueids)
        {
            return context.SP_RentPayment(UserID, i_InvoiceID, v_AccountNumber, v_AccountType, v_TransId, v_StatusCode, v_StatusText, v_CustomerIP, i_Total, v_Comments, v_Address, v_RentDueids).FirstOrDefault();
        }

        public static IQueryable<vProductOrder> ProductOrders()
        {
            return from s in context.vProductOrders select s;
        }

        public static IQueryable<vRentPayment> RentPayments()
        {
            return from s in context.vRentPayments select s;
        }

        public static List<TenantOutstanding> TenantOutstanding(int ownerID)
        {
            return context.SP_TenantOutstanding(ownerID).ToList();
        }
        public static IQueryable<ProductOrder> ProductOrderIds()
        {
            return from s in context.ProductOrders select s;
        }
    }
}
