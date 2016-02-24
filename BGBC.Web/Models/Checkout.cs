    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class Checkout
    {
        [DataType(DataType.Password)]
        [Display(Name = "Choose Password")]
        public string ChoosePassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("ChoosePassword", ErrorMessage = "New Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

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
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string BillingAddress { get; set; }

        [Display(Name = "Use Property Address")]
        public bool BillingAddressSame { get; set; }

        [Display(Name = "Address 2")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string BillingAddress_2 { get; set; }

        [Required]
        [Display(Name = "City")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string BillingCty { get; set; }

        [Required]
        [Display(Name = "State")]
        [DataType(DataType.Text)]
        public string BillingState { get; set; }

        [Required]
        [Display(Name = "Zip")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = " Zip code must be 5 Digits Only")]
        public string BillingZip { get; set; }

        [Display(Name = "Save Billing Address")]
        public bool SaveBillingAddress { get; set; }

        [Display(Name = "Same as Billing Address")]
        public bool ServiceBillingAddressSame { get; set; }
      
        [Display(Name = "Address")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z0-9\s.\-]+$", ErrorMessage = "Use alphanumeric characters only")]
        public string ServiceAddress { get; set; }

        [Display(Name = "Address 2")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z0-9\s.\-]+$", ErrorMessage = "Use alphanumeric characters only")]
        public string ServiceAddress_2 { get; set; }

        [Display(Name = "City")]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string ServiceCty { get; set; }

        [Display(Name = "State")]
        [DataType(DataType.Text)]
        public string ServiceState { get; set; }
        
        [Display(Name = "Zip")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = " Zip code must be 5 Digits Only")]
        public string ServiceZip { get; set; }

        public string PaymentMethod { get; set; }

        [Display(Name = "Card Number")]
        [DataType(DataType.CreditCard)]
        [MaxLength(16)]
        public string CardNo { get; set; }

        public string CardExpMon { get; set; }

        public string CardExpYear { get; set; }

        public BGBC.Core.CreditCardTypeType CardType { get; set; }

        [MaxLength(4)]
        [DataType(DataType.Password)]
        public string CVV { get; set; }

        [Display(Name = "Bank Account Number")]
        public string BankAccountNumber { get; set; }

        [Display(Name = "Bank Account Type")]
        public string BankAccountType { get; set; }

        [Display(Name = "Bank Routing Number")]
        public string BankRoutingNumber { get; set; }

        [Display(Name = "Save Card")]
        public bool SaveCard { get; set; }

        [DataType(DataType.Text)]
        public string Comments { get; set; }

        [Display(Name = "Order Total:")]
        [DataType(DataType.Currency)]

        public string ProductIds { get; set; }

        [DataType(DataType.Currency)]
        public decimal OrderTotal { get; set; }

        public List<OrderSummary> OrderList { get; set; }
    }

    public class OrderSummary
    {
        public int ProductID { get; set; }
        public string Item { get; set; }

        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public short Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        public string rowtype { get; set; }
    }
}