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
    public class DishesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DishService _dishService;
        private readonly IngredientService _ingredientService;

        public DishesController(ApplicationDbContext context, DishService dishService, IngredientService ingredientService)
        {
            _context = context;
            _dishService = dishService;
            _ingredientService = ingredientService;
        }

        public IActionResult Menu()
        {
            var categoryDish = _context.Categories.Include(x => x.Dishes).ThenInclude(z => z.DishIngredients)
                .ThenInclude(y => y.Ingredient).ToList();

            return View(categoryDish);
        }


        // GET: Dishes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dishes.ToListAsync());
        }

        //public IActionResult Menu()
        //{

        //    return View(_context.Dishes.Include(d => d.DishIngredients).ThenInclude(di => di.Ingredient).ToList());
        //}

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .SingleOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // GET: Dishes/Create
        public IActionResult Create(int? id, Dish dish)
        {
            ViewData["categoryList"] = new SelectList(_context.Categories, "CategoryId", "Name", dish.CategoryId);

            return View();
        }
        // POST: Dishes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DishId,Name,Price, CategoryId")] Dish dish, IFormCollection iFormCollection)
        {
            if (ModelState.IsValid)
            {
                foreach (var i in _ingredientService.GetIngredients())
                {
                    var dishIngredient = new DishIngredient()
                    {
                        Ingredient = i,
                        Dish = dish,
                        Enabled = iFormCollection.Keys.Any(x => x == $"IngredientBox-{i.IngredientId}")

                    };
                    _context.DishIngredients.Add(dishIngredient);

                }

               // _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Menu));
            }
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.Include(m => m.DishIngredients).SingleOrDefaultAsync(x => x.DishId == id);

            if (dish == null)
            {
                return NotFound();
            }
            return View(dish);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DishId,Name,Price, CategoryId")] Dish dish, IFormCollection form)
        {
            if (id != dish.DishId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { 
                    var dishToEdit = _context.Dishes.Include(x => x.DishIngredients).FirstOrDefault(x => x.DishId == id);

                    foreach (var ingre in dishToEdit.DishIngredients)
                    {

                        _context.Remove(ingre);
                    }
                    _context.SaveChanges();

                    foreach (var i in _ingredientService.GetIngredients())
                    {
                        var dishIngredient = new DishIngredient()
                        {
                            Ingredient = i,
                            Dish = dish,
                            Enabled = form.Keys.Any(x => x == $"IngredientBox-{i.IngredientId}")

                        };
                        _context.DishIngredients.Add(dishIngredient);

                    }

                    //_context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.DishId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Menu));
            }
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .SingleOrDefaultAsync(m => m.DishId == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dish = await _context.Dishes.SingleOrDefaultAsync(m => m.DishId == id);
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Menu));
        }

        private bool DishExists(int id)
        {
            return _context.Dishes.Any(e => e.DishId == id);
        }
    }
}
