using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlinePizza.Data;
using OnlinePizza.Models;

namespace OnlinePizza.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> OrderPayment()
        {
            //var applicationDbContext = _context.Orders.Include(o => o.Cart).ThenInclude(c => c.Items);
            //return View(await applicationDbContext.ToListAsync());

            var cartId = (int)HttpContext.Session.GetInt32("CartSession");
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("User"))
            {
                var newUser = new OrderViewModel
                {
                    User = new ApplicationUser
                    {

                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        PostalCode = user.PostalCode,
                        City = user.City,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    }
                };
                return View(newUser);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> OrderPayment(OrderViewModel orderVM)
        {
            var cartId = (int)HttpContext.Session.GetInt32("CartSession");
            Cart cart;
            List<CartItem> cartItems;

            cart = _context.Carts
                .Include(i => i.Items)
                .ThenInclude(x => x.CartItemIngredients)
                .ThenInclude(ig => ig.Ingredient)
                .Include(i => i.Items)
                .ThenInclude(ci => ci.Dish)
                .SingleOrDefault(x => x.CartId == cartId);

            cartItems = cart.Items;

            if (User.IsInRole("User"))
            {
                var user = await _userManager.GetUserAsync(User);

                var newModel = new OrderViewModel
                {
                    User = new ApplicationUser
                    {
                        FirstName = orderVM.Order.FirstName,
                        LastName = orderVM.Order.LastName,
                        Address = orderVM.Order.ShippingAddress,
                        PostalCode = orderVM.Order.PostalCode,
                        City = orderVM.Order.City,
                        Email = orderVM.Order.Email,
                        PhoneNumber = orderVM.Order.PhoneNumber

                    },
                    Order = new Order()
                    {
                        CartId = cartId,
                        Cart = cart,
                        CartItem = cartItems,
                        CardName = orderVM.Order.CardName,
                        CardNumber = orderVM.Order.CardNumber,
                        MMYY = orderVM.Order.MMYY,
                        CVC = orderVM.Order.CVC
                    }
                };
                return RedirectToAction("OrderConfirmation", "OrderConfirmations", newModel);
            }
            else
            {
                var newPayment = new Order()
                {
                    OrderId = orderVM.Order.OrderId,
                    FirstName = orderVM.Order.FirstName,
                    LastName = orderVM.Order.LastName,
                    ShippingAddress = orderVM.Order.ShippingAddress,
                    PostalCode = orderVM.Order.PostalCode,
                    Email = orderVM.Order.Email,
                    PhoneNumber = orderVM.Order.PhoneNumber,
                    City = orderVM.Order.City,
                    CartId = cartId,
                    Cart = cart,
                    CartItem = cartItems,
                    CardName = orderVM.Order.CardName,
                    CardNumber = orderVM.Order.CardNumber,
                    MMYY = orderVM.Order.MMYY,
                    CVC = orderVM.Order.CVC
                };

                await _context.Orders.AddAsync(newPayment);
                await _context.SaveChangesAsync();

                return RedirectToAction("OrderConfirmation", "OrderConfirmations", newPayment);
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cart)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderId,UserId,CartId")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
        //    return View(order);
        //}

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,CartId")] Order order)
        //{
        //    if (id != order.OrderId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(order);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderExists(order.OrderId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", order.CartId);
        //    return View(order);
        //}

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cart)
                .SingleOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var order = await _context.Orders.SingleOrDefaultAsync(m => m.OrderId == id);
        //    _context.Orders.Remove(order);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
