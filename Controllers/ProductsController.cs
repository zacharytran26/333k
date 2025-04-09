using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tran_Zachary_HW5.DAL;
using Tran_Zachary_HW5.Models;

namespace Tran_Zachary_HW5.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Suppliers)       // Include suppliers in the details view
                .Include(p => p.OrderDetails)    // Include order details in the details view
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Suppliers = GetAllSuppliers();
            return View(new ProductViewModel());
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = model.Product;

                // Associate selected suppliers with the product
                if (model.SelectedSupplierIds != null && model.SelectedSupplierIds.Any())
                {
                    product.Suppliers = _context.Suppliers
                        .Where(s => model.SelectedSupplierIds.Contains(s.SupplierID))
                        .ToList();
                }

                // Add the product to the database and save changes
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If validation fails, re-populate the supplier list
            model.Suppliers = GetAllSuppliers();
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id, Supplier supplier)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Suppliers)
                .Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Product = product,
                SelectedSupplierIds = product.Suppliers.Select(s => s.SupplierID).ToList(),
            };
            ViewBag.Suppliers = GetAllSuppliers(model.SelectedSupplierIds);
            return View(model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.Product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var productToUpdate = await _context.Products
                    .Include(p => p.Suppliers)
                    .Include(p => p.OrderDetails)
                    .FirstOrDefaultAsync(p => p.ProductID == id);

                if (productToUpdate != null)
                {
                    productToUpdate.Name = model.Product.Name;
                    productToUpdate.Description = model.Product.Description;
                    productToUpdate.Price = model.Product.Price;
                    productToUpdate.Type = model.Product.Type;

                    // Update suppliers
                    var selectedSupplierIds = new HashSet<int>(model.SelectedSupplierIds);
                    var currentSupplierIds = new HashSet<int>(productToUpdate.Suppliers.Select(s => s.SupplierID));

                    // Remove suppliers that are no longer selected
                    productToUpdate.Suppliers.RemoveAll(s => !selectedSupplierIds.Contains(s.SupplierID));

                    // Add new suppliers that are selected
                    var suppliersToAdd = _context.Suppliers
                        .Where(s => selectedSupplierIds.Contains(s.SupplierID) && !currentSupplierIds.Contains(s.SupplierID))
                        .ToList();

                    productToUpdate.Suppliers.AddRange(suppliersToAdd);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            model.Suppliers = GetAllSuppliers();
            return View(model);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        private MultiSelectList GetAllSuppliers(List<int> selectedSupplierIds = null)
        {
            var suppliers = _context.Suppliers.OrderBy(s => s.SupplierName).ToList();
            return new MultiSelectList(suppliers, "SupplierID", "SupplierName");
        }
    }
}
