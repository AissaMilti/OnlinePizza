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

            var item = _context.CartItems
                .Include(d => d.Cart)
                .ThenInclude(s => s.Items)
                .ThenInclude(f => f.Dish)
                .ThenInclude(t => t.DishIngredients)
                .ThenInclude(r => r.Ingredient)
                .ThenInclude(g => g.CartItemIngredients)
                .SingleOrDefault(y => y.CartItemId == id);

            ViewBag.DishName = item.Dish.Name;

            return View(item);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, IFormCollection formCollection, [Bind("CartItemId, DishId")] CartItem cartItem)
        {

            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var itemToEdit = _context.CartItems
                        .Include(ci => ci.CartItemIngredients)
                        .ThenInclude(cii => cii.Ingredient)
                        .Include(d => d.Dish)
                        .ThenInclude(di => di.DishIngredients)
                        .ThenInclude(dii => dii.Ingredient) 
                        .SingleOrDefault(m => m.CartItemId == id);

                    foreach (var cartItemIngredient in itemToEdit.CartItemIngredients)
                    {
                        _context.Remove(cartItemIngredient);
                    }
                    _context.SaveChangesAsync();
                    itemToEdit.CartItemIngredients = new List<CartItemIngredient>();

                    foreach (var ingredient in _ingredientService.GetIngredients())
                    {
                        var cartItemIngredient = new CartItemIngredient
                        {
                            Ingredient = ingredient,
                            Enabled = formCollection.Keys.Any(x => x == $"IngredientBox-{ingredient.IngredientId}")
                            ,
                            CartItemIngredientPrice = ingredient.Price
                        };
                        itemToEdit.CartItemIngredients.Add(cartItemIngredient);
                    }

                    _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cartItem.CartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return View("Index");
           
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
