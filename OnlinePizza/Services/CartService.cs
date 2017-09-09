using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite.Internal.UrlActions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OnlinePizza.Data;
using OnlinePizza.Models;

namespace OnlinePizza.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;


        public CartService(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _context = context;
        }



        public List<CartItem> GetCartItems(HttpContext httpContext)
        {
           
            var dish = _context.CartItems
                .Include(c => c.Dish)
                .ThenInclude(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .ToList();
        
       
            return dish;
        }


        public List<CartItem> AddToCart(int dishId, HttpContext httpContext)
        {
            

            if (dishId == null)
            {
                //Gör något här!

                //return NotFound();
            }

            var dish = _context.Dishes
                .Include(d => d.DishIngredients)
                .ThenInclude(di => di.Ingredient)
                .FirstOrDefault(m => m.DishId == dishId);

            var cartID = httpContext.Session.GetInt32("CartSession");

            Cart cart;

            if (httpContext.Session.GetInt32("CartSession") == null)
            {
                cart = new Cart
                {
                    Items = new List<CartItem>()
                };

                _context.Carts.Add(cart);
                _context.SaveChanges();
                httpContext.Session.SetInt32("CartSession", cart.CartId);
                cartID = cart.CartId;
            }

            cart = _context.Carts.Include(c => c.Items)
                .ThenInclude(ci => ci.Dish)
                .ThenInclude(d => d.DishIngredients)
                .SingleOrDefault(c => c.CartId == cartID);

            CartItem cartItem = new CartItem
            {
                Dish = dish,
                Cart = cart,
                CartItemIngredients = new List<CartItemIngredient>(),
                Quantity = 1
            };

            foreach (var item in dish.DishIngredients)
            {
                var cartItemIngredient = new CartItemIngredient
                {
                    Ingredient = item.Ingredient,
                    CartItem = cartItem
                };
                cartItem.CartItemIngredients.Add(cartItemIngredient);
            }
            cart.Items.Add(cartItem);

            _context.SaveChanges();

            return cart.Items;
        }

        public void DeleteCartItem(Guid? id)
        {
            var cart = _context.CartItems.SingleOrDefault(m => m.CartItemId == id);

            _context.CartItems.Remove(cart);
            _context.SaveChangesAsync();
        }


    }


}


