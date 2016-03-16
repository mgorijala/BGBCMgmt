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
    
    public partial class Property
    {
        public Property()
        {
            this.RentDues = new HashSet<RentDue>();
            this.RentPayments = new HashSet<RentPayment>();
            this.Tenants = new HashSet<Tenant>();
        }
    
        public int PropertyID { get; set; }
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public Nullable<short> RentDueDay { get; set; }
        public Nullable<short> GracePeriod { get; set; }
        public Nullable<decimal> Penalty { get; set; }
        public Nullable<decimal> DailyPenalty { get; set; }
        public Nullable<short> FinalDueDay { get; set; }
        public bool LeaseDocument { get; set; }
        public Nullable<System.DateTime> Createdon { get; set; }
        public Nullable<System.DateTime> Updatedon { get; set; }
        public Nullable<System.DateTime> Deletedon { get; set; }
    
        public virtual User User { get; set; }
        public virtual ICollection<RentDue> RentDues { get; set; }
        public virtual ICollection<RentPayment> RentPayments { get; set; }
        public virtual ICollection<Tenant> Tenants { get; set; }
    }
}
