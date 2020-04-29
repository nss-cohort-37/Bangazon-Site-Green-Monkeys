﻿using System;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Constraints;

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
            var products = from p in _context.Product 
                           select p;
                ////.Where(p => p.UserId == user.Id)
                //.Include(p => p.User)
                //.Include(p => p.ProductType)
                //.ToListAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Title.Contains(searchString));
            }

            return View(await products.ToListAsync());
        }


        // GET: Products/Details/5

        public ActionResult Details(int id)

        {

            return View();

        }



        // GET: Products/Create

        public async Task<ActionResult> Create()

        {
            var ProductTypes = await _context.ProductType
                .Select(pt => new SelectListItem()
                {
                    Text = pt.Label,
                    Value = pt.ProductTypeId.ToString() })
                .ToListAsync();


                var productCreateViewModel = new ProductCreateViewModel();

            productCreateViewModel.ProductTypeOptions = ProductTypes;

            return View(productCreateViewModel);

        }



        // POST: ShoppingItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId,File")]ProductCreateViewModel productCreateViewModel)
        {
            
            try
            {
                //get the current user object
                var user = await GetCurrentUserAsync();

                //create a product object from the values of the create view model
                var product = new Product

                {
                    ProductId = productCreateViewModel.ProductId,
                    DateCreated = productCreateViewModel.DateCreated,
                    Price = productCreateViewModel.Price,
                    Title = productCreateViewModel.Title,
                    Description = productCreateViewModel.Description,
                    UserId = user.Id,
                    Quantity = productCreateViewModel.Quantity,
                    ProductTypeId = productCreateViewModel.ProductTypeId,
                    City = productCreateViewModel.City,
                    Active = productCreateViewModel.Active
                };

                if (productCreateViewModel.ImageFile != null && productCreateViewModel.ImageFile.Length > 0) { 

                var fileName = Guid.NewGuid().ToString() + Path.GetFileName(productCreateViewModel.ImageFile.FileName);
                    
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images", fileName);

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
            catch (Exception)
            {
                var productTypes = await _context.ProductType
                    .Select (p => new SelectListItem() { Text = p.Label,
                    Value = p.ProductTypeId.ToString() })
                    .ToListAsync();
                productCreateViewModel.ProductTypeOptions = productTypes;

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