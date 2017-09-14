using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePizza.Models
{
    public class EditCartIngredientVM
    {
        public int DishId { get; set; }
        public Dish Dish { get; set; }
        public List<SelectedCartItemIngredient> SelectedCartItemIngredients { get; set; }
        public int CartItemId { get; set; }
    }
}
