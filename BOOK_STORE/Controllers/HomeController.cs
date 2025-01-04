    using BOOK_STORE.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace BOOK_STORE.Controllers
    {
        public class HomeController : Controller
        {
            private readonly BookStoreDbContext context;

            public HomeController(BookStoreDbContext context)
            {
                this.context = context;
            }

            // GET: /Home/Index
            public IActionResult Index()
            {
                // Fetch featured books
                var featuredBooks = context.Books
                    .Take(4) // Limit to 4 books
                    .Select(book => new
                    {
                        book.Id,
                        book.Title,
                        book.Price,
                        book.ISBN,
                        AuthorName = book.Author.Name
                    })
                    .ToList();

                ViewBag.FeaturedBooks = featuredBooks;
                // Fetch Categories
                ViewBag.Categories = context.Categories
                                             .Select(c => new
                                             {
                                                 c.Id,
                                                 c.Name,
                                                 BookCount = c.Books.Count()
                                             })
                                             .ToList();
            // Fetch customer reviews
            var reviews = context.Reviews
                .Include(r => r.User) // Ensure the User data is loaded
                .Take(4) // Limit to 4 reviews to display
                .ToList();
            ViewBag.review = reviews;
          

            // Check if the user session exists
            if (HttpContext.Session.GetString("UserSession") != null)
                {
                    ViewBag.MySession = HttpContext.Session.GetString("UserSession").ToString();
                }
                else
                {
                    return RedirectToAction("Login");
                }

                return View();
            }

            // GET: /Home/Login
            public IActionResult Login()
            {
                // If user is already logged in, redirect to Index
                if (HttpContext.Session.GetString("UserSession") != null)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View();
            }

            // POST: /Home/Login
            [HttpPost]
            public IActionResult Login(User user)
            {
                var User = context.Users
                                  .Where(x => x.Email == user.Email && x.PasswordHash == user.PasswordHash && x.Role == "Customer")
                                  .FirstOrDefault();
                if (User != null)
                {
                
                        // Check if the role is "ADMIN" and prevent login on the Home page
                        if (User.Role == "ADMIN")
                        {
                            ViewBag.Message = "Admins are not allowed to log in from this page.";
                            return View();
                        }
                        // Set session data
                        HttpContext.Session.SetInt32("UserId", User.Id); // Store user ID in session
                    HttpContext.Session.SetString("UserSession", User.FullName); // Store user full name in session
                    return RedirectToAction(nameof(Index));
                }

                ViewBag.Message = "Login failed";
                return View();
            }

            // GET: /Home/Logout
            public IActionResult Logout()
            {
                // Remove user session and redirect to login page
                HttpContext.Session.Remove("UserSession");
                HttpContext.Session.Remove("UserId");
                return RedirectToAction(nameof(Login));
            }

            public async Task<IActionResult> Details(int id)
            {
                var category = await context.Categories
                                             .Include(c => c.Books)
                                             .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            public IActionResult Category(int id)
            {
                var books = context.Books
                                    .Where(b => b.CategoryId == id)
                                    .ToList();
                return View(books);
            }
            [HttpGet]
            public IActionResult Search(string searchString)
            {
                // Check if searchString is not null or empty
                if (string.IsNullOrWhiteSpace(searchString))
                {
                    // If no search string, return all books or redirect
                    return View("Search", context.Books.ToList());
                }

                // Search books by Title, ISBN, or Description
                var results = context.Books
                    .Where(b => b.Title.Contains(searchString)
                             || b.ISBN.Contains(searchString)
                             || b.Description.Contains(searchString))
                    .ToList();

                return View("Search", results);
            }

        public async Task<IActionResult> Review()
        {
            var bookStoreDbContext = context.Reviews.Include(r => r.Book).Include(r => r.User);
            return View(await bookStoreDbContext.ToListAsync());
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Developer()
        {
            return View();
        }

        // POST: Author/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addAuthor(Author author)
        {
            if (ModelState.IsValid)
            {
                context.Author.Add(author);
                context.SaveChanges();

                return RedirectToAction(nameof(Index)); // Redirect to the Index page
            }

            return View(author);
        }
        public IActionResult Register()
        {
          
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                context.Add(user);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        // GET: Account/Profile
        public IActionResult Profile()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "Home"); // Redirect if not logged in
            }

            // Fetch user information
            var user = context.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.OrderItems)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Pass user data to the view
            return View(user);
        }

        public IActionResult Privacy()
            {
                return View();
            }
        }
    }
