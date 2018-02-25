using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentApp.Models.Payments
{
    public class PaymentInfo
    {
        [Required]
        [RegularExpression(@"^\d{3}-?\d{3}$", ErrorMessage ="The BSB Number shall be a 6 digit number")]
        [Display(Name = "BSB Number")]
        public string BSBNumber { get; set; }

        [Required]
        [RegularExpression(@"^(\d){9,12}$", ErrorMessage = "The Account Number shall be 9-12 digit number")]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Required]
        public int Reference { get; set; }

        [Required]
        public double Amount { get; set; }

        public string Description { get; set; }
    }
}
