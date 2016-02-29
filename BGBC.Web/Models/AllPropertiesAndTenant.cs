using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class AllPropertiesAndTenant
    {
        public string Pname { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string state { get; set; }
        public string Zip { get; set; }
        public List<TenantRentPay> tenantRentPay { get; set; }
        
    }

    public class TenantRentPay
    {
        public int? tuserid { get; set; }
        public string tname { get; set; }
        public List<BGBC.Model.vRentPayment> RentPayment { get; set; }
    }
}