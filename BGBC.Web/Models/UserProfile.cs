using BGBC.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class UserProfile
    {
        public int UserID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        [Display(Name = "Confirm Email")]
        [DataType(DataType.EmailAddress)]
        [Compare("Email", ErrorMessage = "Email and Confirm Email must match")]
        public string ConfirmEmail { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        [Display(Name = "Alternate Email Address")]
        public string AltEmail { get; set; }

        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        [Display(Name = "Confirm Alternate Email Address")]
        [Compare("AltEmail", ErrorMessage = "Confirm AltEmail dose not match.")]
        public string ConfirmAltEmail { get; set; }

        [Display(Name = "Created at")]
        public Nullable<System.DateTime> Createdon { get; set; }

        [Display(Name = "Last Modified date")]
        public Nullable<System.DateTime> Updatedon { get; set; }

        public Profile ProfileInfo { get; set; }

        //Mohan
        //[Display(Name = "Payment Method")]
        //public string PaymentMethod { get; set; }

        //[Display(Name = "Card Number")]
        //[DataType(DataType.CreditCard)]
        //[MaxLength(16)]
        //public string CardNo { get; set; }

        //public string CardExpMon { get; set; }

        //public string CardExpYear { get; set; }

        //public BGBC.Core.CreditCardTypeType CardType { get; set; }

        //[MaxLength(4)]
        //[DataType(DataType.Password)]
        //public string CVV { get; set; }

        //[Display(Name = "Bank Account Number")]
        //public string BankAccountNumber { get; set; }

        //[Display(Name = "Bank Account Type")]
        //public string BankAccountType { get; set; }

        //[Display(Name = "Bank Routing Number")]
        //public string BankRoutingNumber { get; set; }

        //[Display(Name = "Automatically charge My account each month when my rent is due.")]
        //public bool ChargeAccount { get; set; }

        //[Display(Name = "Save Card")]
        //public bool savecard { get; set; }

        //[DataType(DataType.Currency)]
        //[Display(Name = "Monthly Rent :")]
        //public Decimal MonthlyRent { get; set; }

        //[DataType(DataType.Currency)]
        //[Display(Name = "Service Fee(10.75%) : ")]
        //public Decimal ServiceFee { get; set; }

        //[DataType(DataType.Currency)]
        //[Display(Name = "Total Monthly charge :")]
        //public Decimal TotalCharges { get; set; }
    }

    public class TenantProfile : UserProfile
    {
        public UserReference Ref1 { get; set; }
        public UserReference Ref2 { get; set; }
    }

    public class TenantInfo : UserProfile
    {
        [Required]
        [Display(Name = "Tenant Rent Amount")]
        [DataType(DataType.Currency)]
        public Nullable<decimal> RentAmount { get; set; }

        [Required]
        [Display(Name = "Tenant Rent-Final Month Due")]
        public Nullable<System.DateTime> FinalDueDate { get; set; }

        [Display(Name = "Deposit Amount")]
        public Nullable<decimal> Deposit { get; set; }

        [Display(Name = "Deposit Due Date")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DepostDueDate { get; set; }


        [Display(Name = "Pet Deposit Due")]
        public bool PetDepositDue { get; set; }

        [Display(Name = "Pet Deposit Amount")]
        public Nullable<decimal> PetDeposit { get; set; }

        public string PropertyName { get; set; }

        public int PropertyID { get; set; }
        public int TenantID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Property Manager Name")]
        public string OwnerName { get; set; }

        [Display(Name = "Property Address")]
        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        [DataType(DataType.Text)]
        public string State { get; set; }

        [DataType(DataType.PostalCode)]
        public string Zip { get; set; }
    }

}