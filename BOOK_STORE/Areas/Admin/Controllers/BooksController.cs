using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BOOK_STORE.Models;
using Microsoft.Extensions.Hosting;

namespace BOOK_STORE.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly BookStoreDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public BooksController(BookStoreDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Books
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var bookStoreDbContext = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .OrderBy(b => b.Title);

            return View(await bookStoreDbContext.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
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

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookView productTable)
        {
            if (productTable.Photo != null)
            {
                string fileName = "";
                string folder = Path.Combine(_environment.WebRootPath, "images");
                fileName = Guid.NewGuid().ToString() + "_" + productTable.Photo.FileName;
                string filepath = Path.Combine(folder, fileName);
                productTable.Photo.CopyTo(new FileStream(filepath, FileMode.Create));

                // Ensure that AuthorId is valid, use the correct AuthorId from the form
                var authorId = productTable.AuthorId; // Assuming `productTable.AuthorId` is what you need
                var categoryId = productTable.CategoryId; // Assuming `productTable.CategoryId` is correctly set

                // Create a Book object and assign the Author and Category using their IDs
                Book p = new Book()
                {
                    Id = productTable.Id,
                    Title = productTable.Title,
                    Description = productTable.Description,
                    Price = productTable.Price,
                    StockQuantity = productTable.StockQuantity,
                    AuthorId = authorId, // Set the correct AuthorId
                    CategoryId = categoryId, // Set the correct CategoryId
                    ISBN = fileName
                };

                _context.Books.Add(p);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(productTable);
        }



        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", book.AuthorId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);

            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["AuthorId"] = new SelectList(_context.Author, "Id", "Name", book.AuthorId);
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);

                _context.Update(book);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
             
            }

            return View(book);
        }
        

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                DeletePhoto(book.ISBN);
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private Book MapToBook(BookView bookView, string fileName)
        {
            return new Book
            {
                Id = bookView.Id,
                Title = bookView.Title,
                Price = bookView.Price,
                Description = bookView.Description,
                StockQuantity = bookView.StockQuantity,
                CategoryId = bookView.CategoryId,
                AuthorId = bookView.AuthorId,
                ISBN = fileName
            };
        }

        private bool ValidatePhoto(IFormFile photo, out string errorMessage)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(photo.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                errorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed.";
                return false;
            }

            if (photo.Length > 2 * 1024 * 1024)
            {
                errorMessage = "File size should not exceed 2MB.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private string SavePhoto(IFormFile photo)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(photo.FileName);
            string folder = Path.Combine(_environment.WebRootPath, "images");
            string filePath = Path.Combine(folder, fileName);

            Directory.CreateDirectory(folder); // Ensure the directory exists
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            return fileName;
        }

        private void DeletePhoto(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            string filePath = Path.Combine(_environment.WebRootPath, "images", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

    }
}
