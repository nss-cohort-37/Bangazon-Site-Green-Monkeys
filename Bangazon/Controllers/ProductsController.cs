using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<ActionResult> Index(string searchString, string filter)
        {
            var user = await GetCurrentUserAsync();
            List<Product> products;
                  //.Where(p => p.UserId == user.Id)
                  //.Include(p => p.User)
                  //.Include(p => p.ProductType);
                  //.ToListAsync();

            switch (filter)
            {
                case "Sporting Goods":
                    products = await _context.Product
                        //.Where(ti => ti.UserId == user.Id)
                        .Where(ti => ti.ProductTypeId == 1)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Appliances":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 2)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Tools":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 3)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                        break;
                case "Games":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 4)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Music":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 5)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Health":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 6)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Outdoors":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 7)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Beauty":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 8)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Shoes":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 9)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Automotive":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 10)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;                    
                case "All":
                    products = await _context.Product
                        //.Where(ti => ti.UserId == user.Id)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                default:
                    products = await _context.Product
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
            }

            if (searchString != null)
            {
                var filteredProducts = _context.Product.Where(s => s.Title.Contains(searchString) || s.City.Contains(searchString));
                return View(filteredProducts);
            };

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var product = await _context.Product
               //.Where(p => p.UserId == user.Id)
               .Include(p => p.User)
               .Include(p => p.ProductType)
               .FirstOrDefaultAsync(p => p.ProductId == id);

            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Products/Delete/5
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