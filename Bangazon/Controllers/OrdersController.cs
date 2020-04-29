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

            // Check if the user is logged in, if they aren't, return 401
            if (user == null)
            {
               return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            //if they are and the filter is cart, show them their cart
            else if (filter == "cart")
            {
                // build the item as a view model so we can show more information
                var viewModel = new OrderDetailViewModel();

                // Grab the order and all of it's products for the order that has no payment yet
                var order = await _context.Order
                                    .Where(o => o.UserId == user.Id)
                                    .Include(u => user.PaymentTypes)
                                    .Include(u => u.OrderProducts)
                                    .ThenInclude(op => op.Product)
                                    .FirstOrDefaultAsync(o => o.PaymentType == null);

                



                    // if order comes back empty return an empty cart page
                    if (order.OrderProducts.Count == 0)
                {

                    return RedirectToAction(nameof(EmptyCart));
                }


                //build the individual lines of products in the cart to show the quantity and price
                var lineItems = order.OrderProducts.Select(op => new OrderLineItem()
                {
                    Product = op.Product,
                    Units = op.Product.Quantity,
                    Cost = op.Product.Price,
                });

                //Sum the cost, store it in the view bag to use on the view as a total price
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


        // GET: Orders History
        public async Task<ActionResult> History(string filter)
        {
            var user = await GetCurrentUserAsync();

            // Check if the user is logged in, if they aren't, return 401
            if (user == null)
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            //if they are and the filter is cart, show them their cart
            else if (filter == "history")
            {
                // build the item as a view model so we can show more information
                var viewModel = new OrderHistoryViewModel();

                // Grab the order and all of it's products for the order that has no payment yet
                var orders = await _context.Order
                                    .Where(o => o.UserId == user.Id)
                                    .Include(u => user.PaymentTypes)
                                    .Include(u => u.OrderProducts)
                                    .ThenInclude(op => op.Product)
                                    .Where(o => o.PaymentType != null).ToListAsync();

            
                //build a list of the individual lines of products in the cart to show the quantity and price

                var detailViewModels = new List<OrderDetailViewModel>();


                foreach (var order in orders)
                {

                    order.PaymentType = _context.PaymentType.FirstOrDefault(pt => pt.PaymentTypeId == order.PaymentTypeId);

                    var lineItems = order.OrderProducts.Select(op => new OrderLineItem()

                    {
                        Product = op.Product,
                        Units = op.Product.Quantity,
                        Cost = op.Product.Price,
                    }); ;
                    var orderViewModel = new OrderDetailViewModel();
                    orderViewModel.LineItems = lineItems;
                    orderViewModel.Order = order;
                    orderViewModel.Order.PaymentType = order.PaymentType;
                    orderViewModel.OrderId = order.OrderId;
                    detailViewModels.Add(orderViewModel);

                    //ViewBag.ordertotal = lineItems.Sum(li => li.Cost);
                    orderViewModel.OrderTotalCost = lineItems.Sum(li => li.Cost);

                }

                

                viewModel.Orders = detailViewModels;

         

                return View(viewModel);

            }
            else
            {
                return NotFound();
            }

        }



        //order summary/confirmation view
        public ActionResult OrderSummary(int id)
        {
            return View();
        }

        public ActionResult EmptyCart(int id)
        {
            return View();
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
            //this is the view to 'edit' the cart in order to complete the order. 
            //all we are doing is adding a payment type to the order. 
            var user = await GetCurrentUserAsync();
            var viewModel = new OrderEditViewModel();
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


                return RedirectToAction(nameof(OrderSummary));
            }
            catch
            {
                return View();
            }
        }

        // GET: Orders/Delete/5 THIS IS DELETING INDIVIDUAL ITEMS FROM AN ORDER
        public async Task<ActionResult> Delete(int id)
        {
            var item = await _context.OrderProduct.Include(i => i.Product).FirstOrDefaultAsync(i => i.ProductId == id);
            return View(item);
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, OrderProduct CartItem)
        {
            try
            {
                CartItem = await _context.OrderProduct.FirstOrDefaultAsync(i => i.ProductId == id);
                _context.OrderProduct.Remove(CartItem);
                await _context.SaveChangesAsync();

                return  this.RedirectToAction("", new { filter = "cart" });
            }
            catch
            {
                return View();
            }
        }

        // GET: Orders/Delete/5  THIS IS DELETING THE ORDER
        public async Task<ActionResult> CancelOrder(int id)
        {
            var order = await _context.Order.FirstOrDefaultAsync(i => i.OrderId == id);
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelOrder(int id, Order Cart)
        {
            try
            {
                DeleteCartItems(id);

                Cart = await _context.Order.FirstOrDefaultAsync(i => i.OrderId == id);
                _context.Order.Remove(Cart);
                await _context.SaveChangesAsync();

                return this.RedirectToAction("", new { filter = "cart" });
            }
            catch
            {
                return View();
            }
        }
  

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        private  void DeleteCartItems(int id)
        {
            var itemsToDelete =  _context.OrderProduct.Where(i => i.OrderId == id).ToList();
            foreach(var item in itemsToDelete)
            {
            _context.OrderProduct.Remove(item);

            }
            _context.SaveChanges();
        }
       
    }
}