//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BGBC.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payment
    {
        public Payment()
        {
            this.RentPayments = new HashSet<RentPayment>();
        }
    
        public int PaymentID { get; set; }
        public Nullable<int> UserID { get; set; }
        public int InvoiceID { get; set; }
        public System.DateTime TransDate { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string TransId { get; set; }
        public string StatusCode { get; set; }
        public string StatusText { get; set; }
        public string CustomerIP { get; set; }
        public string BillAddress { get; set; }
        public decimal Total { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> Createdon { get; set; }
    
        public virtual User User { get; set; }
        public virtual ICollection<RentPayment> RentPayments { get; set; }
    }
}
