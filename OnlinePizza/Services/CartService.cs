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
            List<CartItem> cartItems = new List<CartItem>();
            var cartSession = httpContext.Session.GetInt32("Cart");

            if (cartSession == null)
            {
                var carts = _context.Carts.ToList();
                var newId = httpContext.Session.GetInt32("Cart");
                newId = carts.Count + 1;
                cartItems = new List<CartItem>();

                return null;
            }
            else
            {
                Cart cart = new Cart();

                var cartId = httpContext.Session.GetInt32("Cart");

                cart = _context.Carts.Include(x => x.Items).ThenInclude(z => z.Dish).SingleOrDefault(y => y.CartId == cartId);
                cartItems = cart.Items;

            }

            return cartItems;
        }

        public void AddToCart(HttpContext httpContext, int dishId)
        {
            var cartSession = httpContext.Session.GetInt32("Cart");

            Dish dishToAdd = _context.Dishes.Include(x => x.DishIngredients).ThenInclude(i => i.Ingredient).SingleOrDefault(d => d.DishId == dishId);
            List<CartItemIngredient> cartItemIngredient = new List<CartItemIngredient>();
            CartItem cartItem = new CartItem();

            if (cartSession == null)
            {
                Cart cart = new Cart();
                List<CartItem> cartItems = new List<CartItem>();
                var carts = _context.Carts.ToList();
                int newId = carts.Count + 1;
                var newCartItemID = Guid.NewGuid();

                foreach (var item in dishToAdd.DishIngredients)
                {
                    var newCartItemIngredient = new CartItemIngredient
                    {
                        CartItem = cartItem,
                        CartItemId = newCartItemID,
                        IngredientName = item.Ingredient.Name,
                        CartItemIngredientPrice = item.Ingredient.Price
                    };

                    cartItemIngredient.Add(newCartItemIngredient);
                }

                cartItems.Add(new CartItem { Dish = dishToAdd, CartId = newId, Cart = cart, CartItemIngredients = cartItemIngredient, CartItemId = newCartItemID });

                cart.CartId = newId;
                cart.Items = cartItems;

                httpContext.Session.SetInt32("Cart", newId);

                _context.Carts.Add(cart);
                _context.SaveChangesAsync();

            }
            else
            {
                var cartID = httpContext.Session.GetInt32("Cart");
                Cart cart = _context.Carts.Include(x => x.Items).ThenInclude(z => z.Dish).SingleOrDefault(y => y.CartId == cartID);

                var newCartItemId = Guid.NewGuid();

                foreach (var item in dishToAdd.DishIngredients)
                {
                    var newCartItemIngredient = new CartItemIngredient
                    {
                        CartItem = cartItem,
                        CartItemId = newCartItemId,
                        IngredientName = item.Ingredient.Name,
                        CartItemIngredientPrice = item.Ingredient.Price
                    };

                    cartItemIngredient.Add(newCartItemIngredient);
                }

                cart.Items.Add(new CartItem { CartItemId = newCartItemId, Dish = dishToAdd, Cart = cart, CartId = cart.CartId, CartItemIngredients = cartItemIngredient });

                _context.Carts.Update(cart);
                _context.SaveChangesAsync();

            }
        }
    }
}
