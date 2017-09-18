using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlinePizza.Data;
using OnlinePizza.Models;

namespace OnlinePizza.Controllers
{
    public class OrderConfirmationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public OrderConfirmationsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // GET: OrderConfirmationGuest
        public async Task<ActionResult> OrderConfirmation(int orderId, OrderConfirmation orderConfirm)
        {
            var cartId = (int)HttpContext.Session.GetInt32("CartSession");
            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole("User"))
            {


                var currentGuest = await _context.Orders
                    .Include(x => x.Cart)
                    .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.CartItemIngredients)
                    .ThenInclude(x => x.Ingredient)
                    .Include(x => x.CartItem)
                    .ThenInclude(x => x.Dish)
                    .SingleOrDefaultAsync(x => x.OrderId == orderId && x.CartId == cartId);

                var newOrder = new OrderConfirmation
                {
                    Order = currentGuest
                };

                var session = HttpContext.Session;
                session.Remove("CartSession");
                return View();
            }
            else
            {
                var currentGuest = await _context.Orders
                    .Include(x => x.Cart)
                    .ThenInclude(x => x.Items)
                    .ThenInclude(x => x.CartItemIngredients)
                    .ThenInclude(x => x.Ingredient)
                    .Include(x => x.CartItem)
                    .ThenInclude(x => x.Dish)
                    .SingleOrDefaultAsync(x => x.OrderId == orderId && x.CartId == cartId);

                var currentCart = currentGuest.CartItem.ToList();

                var newOrder = new OrderConfirmation
                {
                    Order = currentGuest
                };

                orderConfirm = newOrder;

                var session = HttpContext.Session;
                session.Remove("CartSession");

                return View(newOrder);
            }
        }
    }
}