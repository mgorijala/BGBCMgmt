using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class Payments
    {
        public int PropertyID { get; set; }   
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [Display(Name = "Last Name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string BillingAddress { get; set; }

        [Display(Name = "Address 2")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string BillingAddress_2 { get; set; }

        [Display(Name = "City")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string BillingCty { get; set; }

        [Display(Name = "State")]
        [DataType(DataType.Text)]
        public string BillingState { get; set; }

        [Display(Name = "Zip")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = " Zip code must be 5 Digits Only")]
        public string BillingZip { get; set; }

        [Display(Name = "Save Billing Address")]
        public bool SaveBillingAddress { get; set; }

        [Display(Name = "Use Property Address")]
        public bool UsePropertyAddress { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Card Number")]
        [DataType(DataType.CreditCard)]
        public string CardNo { get; set; }

        [Display(Name = "Card Expiry Month")]
        public string CardExpMon { get; set; }

        [Display(Name = "Card Expiry Year")]
        public string CardExpYear { get; set; }

        [DataType(DataType.Password)]
        public string CVV { get; set; }

        [Display(Name = "Bank Account Number")]
        public string BankAccountNumber { get; set; }

        [Display(Name = "Bank Account Type")]
        public string BankAccountType { get; set; }

        [Display(Name = "Bank Routing Number")]
        public string BankRoutingNumber { get; set; }

        public BGBC.Core.CreditCardTypeType CardType { get; set; }
        [Display(Name = "Save Card")]
        public bool SaveCard { get; set; }

        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string Comments { get; set; }

        [Display(Name = "Order Total:")]
        [DataType(DataType.Currency)]
        public decimal OrderTotal { get; set; }

        public List<Rentdue> TenantRent { get; set; }
    }
}