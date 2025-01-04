using BOOK_STORE.Models;
using Microsoft.AspNetCore.Mvc;

namespace BOOK_STORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly BookStoreDbContext context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(BookStoreDbContext context, ILogger<AdminController> logger)
        {
            this.context = context;
            _logger = logger;
        }

        // GET: /Admin/Login
        public IActionResult Login()
        {
            // Check if admin is already logged in
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        public IActionResult Login(User user)
        {
            var adminUser = context.Users
                .FirstOrDefault(u => u.Email == user.Email && u.PasswordHash == user.PasswordHash && u.Role == "ADMIN");

            if (adminUser != null)
            {
                // Set session for admin
                HttpContext.Session.SetInt32("AdminId", adminUser.Id);
                HttpContext.Session.SetString("AdminSession", adminUser.FullName);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Message = "Login failed. Only admin users can access this panel.";
            return View();
        }

        // GET: /Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminSession");
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction(nameof(Login));
        } 

        // GET: /Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Ensure admin session is valid
            if (HttpContext.Session.GetString("AdminSession") == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                ViewBag.TotalBooks = context.Books?.Count() ?? 0;
                ViewBag.TotalCategories = context.Categories?.Count() ?? 0;
                ViewBag.TotalCategories = context.Author?.Count() ?? 0;
                ViewBag.TotalUsers = context.Users?.Count() ?? 0;
                ViewBag.TotalOrders = context.Order?.Count() ?? 0; // Check Orders table
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading dashboard data.");
                ViewBag.TotalBooks = 0;
                ViewBag.TotalCategories = 0;
                ViewBag.TotalUsers = 0;
                ViewBag.TotalOrders = 0;
            }

            return View();
        }



        // Example: Manage Users
        public IActionResult Users()
        {
            if (HttpContext.Session.GetString("AdminSession") == null)
            {
                return RedirectToAction("Login");
            }

            var users = context.Users.ToList();
            return View(users);
        }
    }
}
