using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BGBC.Web.Models
{
    public class Register
    {

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
        [DataType(DataType.Text)]
        [Display(Name = "User Type")]
        public int UserType { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[A-Za-z0-9._-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,6}", ErrorMessage = "Use valid email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "New Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }

        [MustBeTrue(ErrorMessage = "Please accept the terms & conditions")]
        public bool TermsAccepted { get; set; }


        public class MustBeTrueAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                return value is bool && (bool)value;
            }
        }
    }
}