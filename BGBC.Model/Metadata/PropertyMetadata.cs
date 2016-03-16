using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    class PropertyMetadata
    {
        [Required]
        [Display(Name = "Property Name")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string Name { get; set; }

        [Display(Name = "Property Nickname")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string Nickname { get; set; }

        [Required]
        [Display(Name = "Address")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string Address { get; set; }

        
        [Display(Name = "Address2")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "City")]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "Zip")]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = " Zip code must be 5 Digits Only")]
        public string Zip { get; set; }

        
        [Display(Name = "Rent Due Monthly On...")]
        public Nullable<short> RentDueDay { get; set; }

        
        [Display(Name = "Grace Period")]
        public Nullable<short> GracePeriod { get; set; }

        [Display(Name = "Final Due Day")]
        public Nullable<short> FinalDueDay { get; set; }
        
        [Display(Name = "Late Penalty Charge")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> Penalty { get; set; }

        
        [Display(Name = "Daily Late Penalty Charge")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> DailyPenalty { get; set; }

        [Display(Name = "Lease Document")]
        public Nullable<bool> LeaseDocument { get; set; }


    }
}
