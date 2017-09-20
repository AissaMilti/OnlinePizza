using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnlinePizza.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Firstname")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Lastname")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Card name")]
        public string CardName { get; set; }
        [Required]
       
        [Display(Name = "Card number")]
        public string CardNumber { get; set; }
        [Required]

        [Display(Name = "MMYY")]
        public string MMYY { get; set; }
        [Required]
        [Display(Name = "CVC")]
        public string CVC { get; set; }

        public List<CartItem> CartItem { get; set; }
       
    }
}
