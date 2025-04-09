using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Tran_Zachary_HW5.DAL;
using Tran_Zachary_HW5.Models;

namespace Tran_Zachary_HW5.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(AppDbContext context, UserManager<AppUser> userManger)
        {
            _context = context;
            _userManager = userManger;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            List<Order> orders;

            // Check if the user is an admin
            if (User.IsInRole("Admin"))
            {
                // Retrieve all orders with details and product information for admins
                orders = await _context.Orders
                    .Include(o => o.OrderDetails)           // Include order details
                        .ThenInclude(od => od.Product)      // Include product details within each order detail
                    .Include(o => o.AppUser)                // Include user information associated with the order
                    .ToListAsync();
            }
            else
            {
                // Retrieve only the logged-in user's orders with details and product information
                orders = await _context.Orders
                    .Include(o => o.OrderDetails)           // Include order details
                        .ThenInclude(od => od.Product)      // Include product details within each order detail
                    .Include(o => o.AppUser)                // Include user information associated with the order
                    .Where(o => o.AppUser.UserName == User.Identity.Name)
                    .ToListAsync();
            }

            return View(orders);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //the user did not specify a registration to view
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order to view!" });
            }

            //find the registration in the database
            Order order = await _context.Orders
                                              .Include(r => r.OrderDetails)
                                              .ThenInclude(r => r.Product)
                                              .Include(r => r.AppUser)
                                              .FirstOrDefaultAsync(m => m.OrderID == id);

            //registration was not found in the database
            if (order == null)
            {
                return View("Error", new String[] { "This registration was not found!" });
            }

            //make sure this registration belongs to this user
            if (User.IsInRole("Customer") && order.AppUser.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "This is not your order!  Don't be such a snoop!" });
            }

            //Send the user to the details page
            return View(order);
        }

        // GET: Orders/Create
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([Bind("OrderNotes")] Order order)
        {
            // Generate the next order number
            order.OrderNumber = Utilities.GenerateNextOrderNumber.GetNextOrderNumber(_context);

            // Set the order date to the current date and time
            order.OrderDate = DateTime.Now;

            // Find and associate the logged-in user with the order
            order.AppUser = await _userManager.FindByNameAsync(User.Identity.Name);

            if (order.AppUser == null)
            {
                return View("Error", new string[] { "User not found." });
            }

            // Add the order to the database
            _context.Add(order);
            await _context.SaveChangesAsync();

            // Redirect to OrderDetails/Create to add items to the order
            return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });
        }


        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Order order = await _context.Orders
                                              .Include(r => r.OrderDetails)
                                              .ThenInclude(r => r.Product)
                                              .Include(r => r.AppUser)
                                              .FirstOrDefaultAsync(m => m.OrderID == id);
            if (id == null)
            {
                return NotFound();
            }

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderNumber,OrderDate,OrderNotes")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
        public async Task<SelectList> GetAllCustomerUserNamesSelectList()
        {
            //create a list to hold the customers
            List<AppUser> allCustomers = new List<AppUser>();

            //See if the user is a customer
            foreach (AppUser dbUser in _context.Users)
            {
                //if the user is a customer, add them to the list of customers
                if (await _userManager.IsInRoleAsync(dbUser, "Customer") == true)//user is a customer
                {
                    allCustomers.Add(dbUser);
                }
            }

            //create a new select list with the customer emails
            SelectList sl = new SelectList(allCustomers.OrderBy(c => c.Email), nameof(AppUser.UserName), nameof(AppUser.Email));

            //return the select list
            return sl;

        }
    }
}
