using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Bangazon.Models.ProductViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Appliances":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 2)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Tools":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 3)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                        break;
                case "Games":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 4)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Music":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 5)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Health":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 6)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Outdoors":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 7)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Beauty":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 8)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Shoes":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 9)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                case "Automotive":
                    products = await _context.Product
                        .Where(ti => ti.ProductTypeId == 10)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;                    
                case "All":
                    products = await _context.Product
                        //.Where(ti => ti.UserId == user.Id)
                        .Where(p => p.Quantity > 0)
                        .Include(ti => ti.ProductType)
                        .ToListAsync();
                    break;
                default:
                    products = await _context.Product
                        .Where(p => p.Quantity > 0)
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
        public async Task<ActionResult> Create()

        {
            var productCreateViewModel = new ProductCreateViewModel();

            var productTypeOptions = await _context.ProductType
                .Select(pt => new SelectListItem() { 
                    Text = pt.Label, 
                    Value = pt.ProductTypeId.ToString() 
                })
                .ToListAsync();

            
            productCreateViewModel.ProductTypeOptions = productTypeOptions;

            return View(productCreateViewModel);
        }



        // POST: ShoppingItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCreateViewModel productCreateViewModel)
        {
            
            try
            {
                //get the current user object
                var user = await GetCurrentUserAsync();

                var productTypes = await _context.ProductType
                    .Select (p => new SelectListItem() 
                    { 
                        Text = p.Label,
                        Value = p.ProductTypeId.ToString() })

                    .ToListAsync();

                productCreateViewModel.ProductTypeOptions = productTypes;

                //create a product object from the values of the create view model
                var product = new Product

                {
                    DateCreated = productCreateViewModel.DateCreated,
                    Price = productCreateViewModel.Price,
                    Title = productCreateViewModel.Title,
                    Description = productCreateViewModel.Description,
                    UserId = user.Id,
                    Quantity = productCreateViewModel.Quantity,
                    City = productCreateViewModel.City,
                    Active = productCreateViewModel.Active,
                    ProductTypeId = productCreateViewModel.ProductTypeId
                };

                int id = product.ProductId;

                if (productCreateViewModel.ImageFile != null && productCreateViewModel.ImageFile.Length > 0) { 

                var fileName = Guid.NewGuid().ToString() + Path.GetFileName(productCreateViewModel.ImageFile.FileName);
                    
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                product.ImagePath = fileName;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productCreateViewModel.ImageFile.CopyToAsync(stream);
                }
            }
                _context.Product.Add(product);

                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = product.ProductId });
            }
            catch (Exception ex)
            {

                return View(productCreateViewModel);

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