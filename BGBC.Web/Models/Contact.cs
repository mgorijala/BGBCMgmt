using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class Contact
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Account User Name")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]
        public string AccountName { get; set; }


        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use only alpha characters")]
        [Display(Name = "Your Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        [Display(Name = "Contact Email")]
        public string EMail { get; set; }

        [Required]
        [Display(Name = "Contact Phone")]
        public string Phone { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "How can we help you?")]
        [RegularExpression(@"^[ A-Za-z0-9_@./#&+-]*$", ErrorMessage = "Use alphanumeric characters only")]

        public string MessageText { get; set; }
    }
}