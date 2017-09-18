using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizza.Models
{
    public class OrderViewModel
    {

        public Order Order { get; set; }
        public ApplicationUser User { get; set; }
        public Cart Cart { get; set; }

    }
}
