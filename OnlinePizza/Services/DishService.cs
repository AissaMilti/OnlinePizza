using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlinePizza.Data;
using OnlinePizza.Models;

namespace OnlinePizza.Services
{
    public class DishService
    {
        private readonly ApplicationDbContext _context;

        public DishService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Dish> GetAllDishes()
        {
            var allDishes = _context.Dishes.ToList();
            return allDishes;
        }

        public List<Category> GetAllCategories()
        {
            var allCategories = _context.Categories.ToList();

            return allCategories;
        }

       
    }
}
