using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.OrderViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Controllers
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
        public async  Task<ActionResult> Index(string filter)
        {
            var user = await GetCurrentUserAsync();

            

            
            // filtering items so we only see our own and not other users
            if (user == null)
            {
               return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            else if (filter == "cart")
            {
                var viewModel = new OrderDetailViewModel();


                var order = await _context.Order
                                    .Where(o => o.UserId == user.Id)
                                    .Include(u => user.PaymentTypes)
                                    .Include(u => u.OrderProducts)
                                    .ThenInclude(op => op.Product)
                                    .FirstOrDefaultAsync(o => o.PaymentType == null);
                if (order == null)
                {
                    return NotFound();
                }


                var lineItems = order.OrderProducts.Select(op => new OrderLineItem()
                {
                    Product = op.Product,
                    Units = op.Product.Quantity,
                    Cost = op.Product.Price,
                });

                ViewBag.TotalPrice = lineItems.Sum(li => li.Cost);
               

                viewModel.LineItems = lineItems;
               
                viewModel.Order = order;
                viewModel.OrderId = order.OrderId;

                return View(viewModel);

            } else
            {
                return NotFound();
            }
            
        }

        // GET: Orders/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
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

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var user = await GetCurrentUserAsync();
            var viewModel = new OrderEditViewModel();
            //var paymentTypes = await _context.PaymentType.Where(pt => pt.UserId == user.Id).ToListAsync();
            var order = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == id);

            var paymentTypes = await _context.PaymentType
               .Select(pt => new SelectListItem()
               {
                   Text = pt.Description,
                   Value = pt.PaymentTypeId.ToString()
               }).ToListAsync();


            viewModel.paymentTypes = paymentTypes;
            viewModel.order =  order;

            if (order.UserId != user.Id)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Order order)
        {
            try
            {
                var user = await GetCurrentUserAsync();
                order.UserId = user.Id;
                order.OrderId = id;
            
                order.DateCompleted = DateTime.Now;
                
                _context.Order.Update(order);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Orders/Delete/5
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
        private async Task<List<Order>> GetUnfulfilledOrders()
        {
            var user = await GetCurrentUserAsync();

            var orders = await _context.Order
                    .Where(o => o.UserId == user.Id)
                    .Include(u => user.PaymentTypes)
                    .Include(u => u.OrderProducts)
                    .ThenInclude(op => op.Product)
                    .Where(o => o.PaymentType == null)
                    .ToListAsync();

            return orders;
        }
    }
}