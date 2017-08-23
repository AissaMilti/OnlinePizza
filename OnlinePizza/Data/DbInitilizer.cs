using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OnlinePizza.Models;

namespace OnlinePizza.Data
{
    public class DbInitializer
    {
        public static void Initialize(UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            var aUser = new ApplicationUser
            {
                UserName = "student@test.com",
                Email = "student@test.com"
            };
            var userResult = userManager.CreateAsync(aUser, "Pa$$w0rd").Result;

            var adminRole = new IdentityRole { Name = "Admin" };
            var roleResult = roleManager.CreateAsync(adminRole).Result;

            var adminUser = new ApplicationUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com"
            };
            var adminUserResult = userManager.CreateAsync(adminUser, "Pa$$w0rd").Result;

            var roleAddedResult = userManager.AddToRoleAsync(adminUser, "Admin").Result;

            if (!context.Dishes.Any())
            {
                var cheese = new Ingredient { Name = "Cheese" };
                var tomato = new Ingredient { Name = "Tomato" };
                var ham = new Ingredient { Name = "Ham" };
                var pasta = new Ingredient {Name = "Pasta"};
                var chicken = new Ingredient {Name = "Chicken"};
                var garlicSauce = new Ingredient {Name = "Garlic Sauce"};

                //Pizza
                var capricciosa = new Dish { Name = "Capricciosa", Price = 80 };
                var margarita = new Dish { Name = "Margarita", Price = 70 };
                var hawaii = new Dish { Name = "Hawaii", Price = 100 };
                var kebabPizza = new Dish {Name = "Kebabpizza", Price = 80};

                //Pasta
                var pastaAlfredo = new Dish {Name = "Pasta Alfredo", Price = 90};

                var margaritaCheese = new DishIngredient { Dish = margarita, Ingredient = cheese };
                var margaritaHam = new DishIngredient { Dish = margarita, Ingredient = ham };

                var hawaiiCheese = new DishIngredient { Dish = hawaii, Ingredient = tomato };

                var capricciosaCheese = new DishIngredient { Dish = capricciosa, Ingredient = ham };
                var capricciosaTomato = new DishIngredient { Dish = capricciosa, Ingredient = tomato };

                var kebabTomato = new DishIngredient {Dish = kebabPizza, Ingredient = tomato};
                var kebabGarlicSauce = new DishIngredient {Dish = kebabPizza, Ingredient = garlicSauce};

                var alfredoChicken = new DishIngredient {Dish = pastaAlfredo, Ingredient = chicken};
                var alfredoPasta = new DishIngredient {Dish = pastaAlfredo, Ingredient = pasta};



                capricciosa.DishIngredients = new List<DishIngredient> { capricciosaCheese, capricciosaTomato };
                margarita.DishIngredients = new List<DishIngredient> { margaritaCheese, margaritaHam };
                hawaii.DishIngredients = new List<DishIngredient> { hawaiiCheese };
                kebabPizza.DishIngredients = new List<DishIngredient>{kebabTomato, kebabGarlicSauce};
                pastaAlfredo.DishIngredients = new List<DishIngredient>{alfredoPasta, alfredoChicken};

                context.AddRange(tomato, ham, cheese);

                context.DishIngredients.AddRange(margaritaCheese, hawaiiCheese, capricciosaCheese, capricciosaTomato, margaritaHam, kebabGarlicSauce, kebabTomato, alfredoPasta, alfredoChicken);
                context.AddRange(capricciosa, margarita, hawaii, kebabPizza, pastaAlfredo);
                context.SaveChanges();
            }
        }
    }
}
