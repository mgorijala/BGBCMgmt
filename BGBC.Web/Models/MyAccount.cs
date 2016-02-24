using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BGBC.Model;
namespace BGBC.Web.Models
{
    public class MyAccount
    {
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        public string Name { get; set; }

        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Display(Name = "Rent Due")]
        public string RentDue { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Rent Amount")]
        public Decimal? RentAmount { get; set; }
        public bool Select { get; set; }

        public List<Rentdue> TenantRent { get; set; }

        public List<RecentPay> RecentPayments { get; set; }
    }

    public class Rentdue
    {
        public int ID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime DueDateDisplay { get { return this.DueDate; } }

        [DataType(DataType.Text)]
        public string Charge { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }
        public bool select { get; set; }

        public int PropertyID { get; set; }
    }


    public class RecentPay
    {
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Display(Name = "Conformation #")]
        public string Conformation { get; set; }

        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")] 
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [DataType(DataType.Text)]
        public string TenantFirstName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [DataType(DataType.Text)]
        public string TenantLastName { get; set; }

        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [DataType(DataType.Text)]
        [Display(Name = "Payment Status")]
        public string PaymentStatus { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }

        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        [DataType(DataType.Text)]
        public string Comment { get; set; }
    }
}