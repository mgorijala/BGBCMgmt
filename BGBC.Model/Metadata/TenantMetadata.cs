using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace BGBC.Model.Metadata
{
    public class TenantMetadata
    {
         
        [Display(Name = "Tenant Rent Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> RentAmount { get; set; }
       
        [Display(Name = "Tenant Rent-Final Month Due")] 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FinalDueDate { get; set; }

        [Display(Name = "Deposit Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> Deposit { get; set; }

        [Display(Name = "Deposit Due Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DepostDueDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Pet Deposit Due")]
        public bool PetDepositDue { get; set; }

        [Display(Name = "Pet Deposit Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> PetDeposit { get; set; }
    }
}
