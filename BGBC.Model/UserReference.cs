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
    
    public partial class UserReference
    {
        public int UserReferenceID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Relationship { get; set; }
        public Nullable<System.DateTime> Createdon { get; set; }
        public Nullable<System.DateTime> Updatedon { get; set; }
    
        public virtual User User { get; set; }
    }
}
