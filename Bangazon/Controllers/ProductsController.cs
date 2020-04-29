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
        public async Task<ActionResult> Index(string searchString)
        {
            var user = await GetCurrentUserAsync();
            var products = _context.Product
                .Where(p => p.UserId == user.Id)
                .Include(p => p.User)
                .Include(p => p.ProductType);
                //.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                var filteredProducts = products.Where(s => s.Title.Contains(searchString));
                return View(filteredProducts);
            };

            return View(await products.ToListAsync());
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

        // GET: Products/AddToCart
        public async Task<ActionResult> AddToCart(int id)
        {
            var user = await GetCurrentUserAsync();
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);

            return View(product);
        }

        // POST: Products/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddToCart(int id, Product product)
        {
            //We are adding a new product to the cart
            try
            {
                var user = await GetCurrentUserAsync();
                //Grabbing the order that doesn't have a payment type yet (i.e. the cart)
                var order = await _context.Order
                               .Where(o => o.UserId == user.Id)
                               .Include(u => user.PaymentTypes)
                               .Include(u => u.OrderProducts)
                               .ThenInclude(op => op.Product)
                               .FirstOrDefaultAsync(o => o.PaymentType == null);
                //create a new empty order product that will be added later
                var orderProduct = new OrderProduct();
              
                //check if the user has an open cart.
                if (order == null)
                {
                    // the user has no open cart so we build a new one

                    var newOrder = new Order();
                    newOrder.UserId = user.Id;
                    
                    //and save it to the database
                    _context.Order.Add(newOrder);
                    await _context.SaveChangesAsync();

                    //then we build the order product relationship
                    orderProduct.ProductId = id;
                    orderProduct.OrderId = newOrder.OrderId;

                    //and add that to the database
                    _context.OrderProduct.Add(orderProduct);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    //the user has a cart so we simply build that Order Product relationship
                    orderProduct.ProductId = id;
                    orderProduct.OrderId = order.OrderId;

                    //and save that to the database
                    _context.OrderProduct.Add(orderProduct);
                    await _context.SaveChangesAsync();
                }
                
                //return the user to the cart
                return this.RedirectToAction("", "Orders", new { filter = "cart" });
            }
            catch
            {
                return View();
            }
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