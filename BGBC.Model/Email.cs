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
    
    public partial class Email
    {
        public int EmailID { get; set; }
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> Createdon { get; set; }
        public Nullable<System.DateTime> Updatedon { get; set; }
    }
}
