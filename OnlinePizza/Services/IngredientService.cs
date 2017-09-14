using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlinePizza.Data;
using OnlinePizza.Models;

namespace OnlinePizza.Services
{
    public class IngredientService
    {
        private readonly ApplicationDbContext _context;

        public IngredientService(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<Models.Ingredient> GetIngredients()
        {
            return _context.Ingredients.ToList();
        }

        public List<CartItemIngredient> GetCartItemIngredients()
        {
            return _context.CartItemIngredients.ToList();
        }

        public string AllIngredientsForDish(int id)
        {
            var ingredients = _context.DishIngredients.Include(x => x.Ingredient)
                .Where(x => x.DishId == id && x.Enabled);

            string allIngredients = "";
            foreach (var ingredient in ingredients)
            {
                allIngredients += ingredient.Ingredient.Name + " ";
            }

            return allIngredients;
        }

        
    }
}
