using System;

namespace OnlinePizza.Models
{
    public class CartItemIngredient
    {
        public int CartItemId { get; set; }
        public CartItem CartItem { get; set; }
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public bool Enabled { get; set; }

        public string IngredientName { get; set; }
        public int CartItemIngredientPrice { get; set; }
    }
}