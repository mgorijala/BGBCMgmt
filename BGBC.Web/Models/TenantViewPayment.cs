using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class TenantViewPayment
    {
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        public string PropertyName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        public string TenantName { get; set; }

        [RegularExpression(@"^[A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")] 
        public string Address { get; set; }

        [RegularExpression(@"^[A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")] 
        public string Address2 { get; set; }

        [DataType(DataType.Text)]
        public string state { get; set; }

        [Required]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = " Zip code must be 5 Digits Only")]
        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }
       
        public List<BGBC.Model.vRentPayment> RentPayment { get; set; }
    }
    }
