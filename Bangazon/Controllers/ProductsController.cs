using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ShoppingItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            var products = await _context.Product
                .Where(p => p.UserId == user.Id)
                .Include(pt => pt.ProductType)
                .ToListAsync();

            return View(products);
        }

        // GET: ShoppingItems/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ShoppingItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShoppingItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                product.UserId = user.Id;

                _context.Product.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShoppingItems/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var item = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);
            var loggedInUser = await GetCurrentUserAsync();

            if (item.UserId != loggedInUser.Id)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: ShoppingItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Product product)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                product.UserId = user.Id;

                _context.Product.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShoppingItems/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShoppingItems/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}