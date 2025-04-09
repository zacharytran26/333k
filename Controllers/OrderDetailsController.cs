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
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index(int? orderID)
        {
            if (orderID == null)
            {
                return View("Error", new String[] { "Please specify an order" });
            }
            List<OrderDetail> ods = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .Where(od => od.Order.OrderID == orderID)
                .ToList();
            return View(ods);
        }

        // GET: OrderDetails/Create
        public async Task<IActionResult> Create(int orderID)
        {
            // Create a new instance of OrderDetail
            var orderDetail = new OrderDetail();

            // Find the order associated with the given OrderID
            //var order = await _context.Orders
            //    .Include(o => o.OrderDetails)
            //    .FirstOrDefaultAsync(o => o.OrderID == orderID);

            Order order = _context.Orders.Find(orderID);

            orderDetail.Order = order;

            if (order == null)
            {
                return NotFound("Order not found");
            }

            // Set the new OrderDetail's order
            orderDetail.Order = order;

            // Populate the ViewBag with a list of existing products
            ViewBag.Products = new SelectList(GetAllProducts(), "ProductID", "Name");

            //OrderDetail orderdetail = new OrderDetail();
            //Order dbOrder = _context.Orders.Find(orderID);
            //orderdetail.Order = dbOrder;
            //ViewBag.AllOrders = GetAllProducts();

            // Pass the OrderDetail object to the view
            return View(orderDetail);

        }

        // Helper method to retrieve all products
        private List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }


        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDetail orderDetail, int SelectedProduct)
        {
            // Repopulate the ViewBag for the dropdown in case validation fails


            // Check if the model state is valid before proceeding
            //if (ModelState.IsValid == false)
            //{
            //    ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductID", "Name");
            //    // Return the view with validation errors
            //    return View(orderDetail);
            //}

            // Find the product associated with the SelectedProduct ID
            Product dbProduct = _context.Products.Find(SelectedProduct);

            // Set the OrderDetail's Product property to the found product
            orderDetail.Product = dbProduct;

            // Find the order associated with this OrderDetail
            Order dbOrder = await _context.Orders.FindAsync(orderDetail.Order.OrderID);

            // Set the OrderDetail's Order property to the found order
            orderDetail.Order = dbOrder;

            // Set the OrderDetail's price to the product's current price
            orderDetail.Price = dbProduct.Price;

            // Calculate the extended price (quantity * price)
            orderDetail.ExtendedPrice = orderDetail.Quantity * orderDetail.Price;

            // Add the OrderDetail to the database and save changes
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            // Redirect to the order details view
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });

        }



        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load the order with associated details, product information, and customer information
            var orderdetail = await _context.OrderDetails
                .Include(o => o.Order)           // Include order details
                    .Include(o => o.Product)      // Include product details within each order detail            
                .FirstOrDefaultAsync(o => o.OrderDetailID == id);

            if (orderdetail == null)
            {
                return NotFound();
            }

            return View(orderdetail);
        }


        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,Quantity,ProductID")] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with validation errors
                return View(orderDetail);
            }

            if (id != orderDetail.OrderDetailID)
            {
                return NotFound();
            }

            OrderDetail ord;
            try
            {
                ord = await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .FirstOrDefaultAsync(od => od.OrderDetailID == id);

                ord.Quantity = orderDetail.Quantity;

                // Recalculate and update ProductPrice and ExtendedPrice
                ord.Price = ord.Product.Price;  // Assume Product has a CurrentPrice property
                ord.ExtendedPrice = ord.Quantity * ord.Price;
                _context.Update(ord);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem editing this record", ex.Message });
            }




            // If there's a problem, reload the Order and Product information for the view
            return RedirectToAction("Index", "Home", new { id = ord.Order.OrderID });
        }



        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }
    }
}
