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
                .ThenInclude(di => di.Ingredient).Include(c => c.CartItemIngredients).ThenInclude(c => c.Ingredient)
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
                    CartItem = cartItem,
                    Enabled = true,
                    CartItemIngredientPrice = item.Ingredient.Price
                };
                cartItem.CartItemIngredients.Add(cartItemIngredient);
            }
            cart.Items.Add(cartItem);

            _context.SaveChanges();

            return cart.Items;
        }

        public int TotalPriceForCart(HttpContext httpContext)
        {

            var cartSession = httpContext.Session.GetInt32("CartSession");

            var totalPrice = 0;

            var cart = _context.Carts.Include(x => x.Items).ThenInclude(x => x.CartItemIngredients).ThenInclude(x => x.Ingredient).FirstOrDefault(x => x.CartId == cartSession);

            var cartIngredients = _context.CartItemIngredients.Include(x => x.Ingredient).Where(x => x.Enabled);

            foreach (var itemPrice in cart.Items)
            {
                totalPrice += itemPrice.Dish.Price;

                totalPrice += itemPrice.CartItemIngredients.Where(s => s.Enabled).Sum(cii => itemPrice.Dish
                    .DishIngredients.Any(di => di.IngredientId == cii.IngredientId) ? 0 : cii.Ingredient.Price);
            }
            return totalPrice;
        }

        public void DeleteCartItem(int? id)
        {
            var cart = _context.CartItems.SingleOrDefault(m => m.CartItemId == id);

            _context.CartItems.Remove(cart);
            _context.SaveChangesAsync();
        }

        public bool HasIngredient(int id, int ingredientId)
        {
            var item = _context.CartItemIngredients.Any(x => x.CartItemId == id && x.IngredientId == ingredientId && x.Enabled);

            return item;
        }


    }


}