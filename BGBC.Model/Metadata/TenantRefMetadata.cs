using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    class TenantRefMetadata
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Contact Name For Landlord/Property Management Company")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string LandlordName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone For Landlord/Property Management Company")]
        public string LandlordPhone { get; set; }

        [Required]
        [Display(Name = "Email For Landlord/Property Management Company")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        public string LandlordEmail { get; set; }

        [Display(Name = "Mailing Address For Landlord/Property Management Company")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string LandlordAddress { get; set; }
    }
}
