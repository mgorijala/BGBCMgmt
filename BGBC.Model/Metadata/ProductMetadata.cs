using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGBC.Model.Metadata
{
    class ProductMetadata
    {
        [Required]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        [StringLength(255)] 
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = " Product Description")]
        [RegularExpression(@"^[^<>!@#%/?*]+$", ErrorMessage = "Invalid Product Description")] 
        //[RegularExpression(@"^[ A-Za-z0-9\r\n_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string Description { get; set; }

        [Required]
        [Range(0, 99999999)]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> Price { get; set; }

        [Display(Name = "Availability Restrictions")]
        public Nullable<bool> isLocal { get; set; }
    }
}
