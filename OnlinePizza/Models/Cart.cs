using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnlinePizza.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int ApplicationId { get; set; }
        public List<CartItem> Items { get; set; }

       
       
    }
}
