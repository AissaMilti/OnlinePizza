using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizza.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string ShippingAddress { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        [Display(Name = "Card name")]
        public string CardName { get; set; }
        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
        [Required]
        [Display(Name = "MMYY")]
        public string MMYY { get; set; }
        [Required]
        [Display(Name = "CVC")]
        public string CVC { get; set; }

        public int UserId { get; set; }
       // public ApplicationUser User { get; set; }
        public Cart Cart { get; set; }
        public int CartId { get; set; }

        public List<Cart> CartList { get; set; }

        public List<CartItem> CartItem { get; set; }
       
        
    }
}
