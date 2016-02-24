using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    public class ProfileMetadata
    {
        [DataType(DataType.EmailAddress)]
        public string AltEmail { get; set; }

        [Display(Name = "Address")]
        public Nullable<bool> BillingAddressSame { get; set; }

        [Display(Name = "Address")]
        public string BillingAddress { get; set; }

        [Display(Name = "Address2")]
        public string BillingAddress_2 { get; set; }

        [Display(Name = "City")]
        public string BillingCty { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "State")]
        public string BillingState { get; set; }

        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip")]
        public string BillingZip { get; set; }

        [Display(Name = "Home Phone")]
        [DataType(DataType.PhoneNumber)]
        public string HomePhone { get; set; }


        [Display(Name = "Work Phone")]
        [DataType(DataType.PhoneNumber)]
        public string WorkPhone { get; set; }


        [Display(Name = "Mobile Phone")]
        [DataType(DataType.PhoneNumber)]
        public string MobilePhone { get; set; }

        [Display(Name = "Fax Phone")]
        [DataType(DataType.PhoneNumber)]
        public string FaxNumber { get; set; }

        public Nullable<bool> SMSOpt { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }


        [Display(Name = "PayPal Email")]
        [DataType(DataType.EmailAddress)]
        public string PaypalEmail { get; set; }

        [Display(Name = "Mailing Address")]
        [DataType(DataType.Text)]
        public string PayoutMailAddress { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mailing Address 2")]
        public string PayoutMailAddress2 { get; set; }

        [Display(Name = "Mailing City")]
        [DataType(DataType.Text)]
        public string PayoutMailCity { get; set; }

        [Display(Name = "Mailing State")]
        [DataType(DataType.Text)]
        public string PayoutMailState { get; set; }

        [Display(Name = "Mailing Zip")]
        [DataType(DataType.PostalCode)]
        public string PayoutMailZip { get; set; }

        public virtual User User { get; set; }
    }
}
