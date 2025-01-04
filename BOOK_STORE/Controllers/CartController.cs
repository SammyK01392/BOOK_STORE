using BOOK_STORE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BOOK_STORE.Controllers
{
    public class CartController : Controller
    {
        private readonly BookStoreDbContext _context;

        public CartController(BookStoreDbContext context)
        {
            _context = context;
        }

        // GET: /Cart/Index
        public IActionResult Index()
        {
            // Fetch all books for display
            var books = _context.Books
                                .Include(b => b.Author)
                                .Include(b => b.Category)
                                .ToList();

            return View(books);
        }

        // GET: /Cart/Cart
        public IActionResult Cart()
        {
            int userId = GetLoggedInUserId(); // Get logged-in user ID
            var cartItems = _context.CartItems
                                    .Include(ci => ci.Book)
                                    .ThenInclude(b => b.Author)
                                    .Where(ci => ci.UserId == userId)
                                    .ToList();

            return View(cartItems); // Pass cart items to view
        }

        // POST: /Cart/AddToCart
        public IActionResult AddToCart(int id)
        {
            int userId = GetLoggedInUserId();

            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null || book.StockQuantity <= 0)
            {
                return NotFound();
            }

            // Check if the item already exists in the cart
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.BookId == id && ci.UserId == userId);
            if (cartItem != null)
            {
                cartItem.Quantity++; // Increase quantity
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = userId,
                    BookId = book.Id,
                    Quantity = 1
                };
                _context.CartItems.Add(cartItem); // Add new item to cart
            }

            book.StockQuantity--; // Reduce stock quantity
            _context.Entry(book).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToAction("Cart");
        }

        // GET: /Cart/Remove
        public IActionResult Remove(int id)
        {
            int userId = GetLoggedInUserId();

            // Eager load the Book related to CartItem
            var cartItem = _context.CartItems
                                   .Include(ci => ci.Book)  // Ensure Book is included
                                   .FirstOrDefault(ci => ci.Id == id && ci.UserId == userId);

            if (cartItem != null && cartItem.Book != null)
            {
                cartItem.Book.StockQuantity += cartItem.Quantity; // Restore stock
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
            else
            {
                // Handle case where cartItem or cartItem.Book is null
                // For example, you could return a message or handle the error appropriately
                return NotFound(); // Or another suitable response
            }

            return RedirectToAction("Cart");
        }
        // POST: /Cart/Checkout


        // POST: /Cart/Checkout
        [HttpPost]
public IActionResult Checkout()
{
    int userId = GetLoggedInUserId();

    // Get user's cart items
    var cartItems = _context.CartItems
                            .Include(ci => ci.Book)
                            .Where(ci => ci.UserId == userId)
                            .ToList();

    if (!cartItems.Any())
    {
        TempData["Message"] = "Your cart is empty.";
        return RedirectToAction("Cart");
    }

    using (var transaction = _context.Database.BeginTransaction())
    {
        try
        {
            // Create an Order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Book.Price),
                Status = "Pending"
            };

            _context.Order.Add(order);
            _context.SaveChanges();

            // Create OrderItems and clear CartItems
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Book.Price
                };

                _context.OrderItems.Add(orderItem);
                _context.CartItems.Remove(cartItem); // Clear from cart
            }

            _context.SaveChanges();
            transaction.Commit();

            return RedirectToAction("Payment", new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            
            return View("Error");
        }
    }
}

        // GET: /Cart/Payment
        public IActionResult Payment(int orderId)
        {
            var order = _context.Order
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Book)
                                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /Cart/Payment
        [HttpPost]
        public IActionResult Payment(int orderId, string paymentMethod)
        {
            var order = _context.Order.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Create a payment record
            var payment = new Payment
            {
                OrderId = orderId,
                PaymentMethod = paymentMethod,
                Amount = order.TotalAmount,
                PaymentDate = DateTime.Now
            };

            _context.Payment.Add(payment);

            // Update the order status to "Paid"
            order.Status = "Paid";
            _context.Entry(order).State = EntityState.Modified;

            _context.SaveChanges();

            return RedirectToAction("Delivery", new { orderId = order.Id });
        }

        // GET: /Cart/Delivery
        public IActionResult Delivery(int orderId)
        {
            var order = _context.Order
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Book)
                                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /Cart/Delivery
        [HttpPost]
        public IActionResult Delivery(int orderId, string deliveryAddress)
        {
            var order = _context.Order.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Finalize the order
            order.Status = "Shipped";
            _context.Entry(order).State = EntityState.Modified;

            // Save delivery details (if needed, create a Delivery model/table)
            // For now, we assume deliveryAddress is recorded elsewhere

            _context.SaveChanges();

            return RedirectToAction("OrderSuccess", new { orderId = order.Id });
        }

        // GET: /Cart/OrderSuccess
        public IActionResult OrderSuccess(int orderId)
        {
            // Fetch the order details to display in the success page
            var order = _context.Order
                                .Include(o => o.OrderItems)
                                .ThenInclude(oi => oi.Book)
                                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        private int GetLoggedInUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                throw new Exception("User is not logged in.");
            }
            return userId.Value;
        }
    }
}
