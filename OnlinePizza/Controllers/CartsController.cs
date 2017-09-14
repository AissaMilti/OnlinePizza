using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlinePizza.Data;
using OnlinePizza.Models;
using OnlinePizza.Services;

namespace OnlinePizza.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CartService _cartService;
        private readonly IngredientService _ingredientService;

        public CartsController(ApplicationDbContext context, CartService cartService, IngredientService ingredientService)
        {
            _context = context;
            _cartService = cartService;
            _ingredientService = ingredientService;
        }

        // GET: Carts
        public IActionResult Index()
        {
            var allCartItems = _cartService.GetCartItems(HttpContext);

            return View(allCartItems);
        }

        public IActionResult AddToCart(int dishId)
        {
            var dishToAdd = _cartService.AddToCart(dishId, HttpContext);

            return RedirectToAction("Index", dishToAdd);
        }



        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .SingleOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create(int id)
        {
            var addedDish = _context.Dishes.FirstOrDefault(d => d.DishId == id);

            return View(addedDish);
        }

        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,ApplicationId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cart);
        }

        // GET: Carts/Edit/5
        public ActionResult Edit(int id)
        {

            //var item = _context.CartItems.Include(ci => ci.CartItemIngredients)
            //    .ThenInclude(cii => cii.Ingredient).SingleOrDefault(c => c.CartItemId == id);

            var item = _context.CartItems.Include(d => d.Cart).ThenInclude(s => s.Items)
                .ThenInclude(f => f.Dish).ThenInclude(t => t.DishIngredients).ThenInclude(r => r.Ingredient).ThenInclude(g => g.CartItemIngredients)
                .SingleOrDefault(y => y.CartItemId == id);

            ViewBag.DishName = item.Dish.Name;

            return View(item);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection formCollection, [Bind("CartId,ApplicationId")] Cart cart)
        {

            var dishItemToEdit = _context.CartItems
                .Include(c => c.CartItemIngredients)
                .ThenInclude(ci => ci.Ingredient)
                .SingleOrDefault(c => c.CartItemId == id);


            var cartId = HttpContext.Session.GetInt32("CartSession");


            foreach (var ingre in dishItemToEdit.CartItemIngredients)
            {
                _context.Remove(ingre);
            }
            _context.SaveChanges();

            foreach (var i in _ingredientService.GetIngredients())
            {
                var cartItemIngredient = new CartItemIngredient()
                {
                    Ingredient = i,
                    IngredientId = i.IngredientId,
                    CartItemIngredientPrice = i.Price,

                    Enabled = formCollection.Keys.Any(x => x == $"IngredientBox-{i.IngredientId}")

                };
                _context.CartItemIngredients.Add(cartItemIngredient);

                var newPrice = 0;

                _context.Update(dishItemToEdit);
                var newCartItem = _context.CartItems.Include(d => d.Dish).ThenInclude(x => x.DishIngredients).SingleOrDefault(c => c.CartItemId == id);

                //var di = _ingredientService.IngredentByDish(dish.DishId);
                foreach (var dishIngredient in newCartItem.Dish.DishIngredients)
                {
                    foreach (var ing in dishItemToEdit.CartItemIngredients)
                    {
                        if (ing.IngredientId != dishIngredient.IngredientId)
                        {
                            newPrice = +5;
                        }
                    }
                }
                dishItemToEdit.DishPrice = dishItemToEdit.DishPrice + newPrice;
                _context.SaveChanges();


            }
            //if (id != cart.CartId)
            //{
            //    return NotFound();
            //}

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            //        _context.Update(cart);
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!CartExists(cart.CartId))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction(nameof(Index));
            //}

            //_cartService.EditCartItemIngredients(id, formCollection);
            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _cartService.DeleteCartItem(id);

            return View("Index");
        }
        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }



    }



}
