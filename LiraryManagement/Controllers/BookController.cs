using LiraryManagement.Data;
using LiraryManagement.Models.Domain;
using LiraryManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LiraryManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDBContext appDBContext;

        public BookController(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // Total number of Books
            int totalBooks = await appDBContext.Books.CountAsync();

            // Calculate the number of records to skip
            int skipCount = (page - 1) * pageSize;

            var paginatedBooks = await appDBContext.Books
                                                     .Where(b => b.IsActive)
                                                     .Include(b => b.Categories)  // Navigation property
                                                     .Include(b => b.Authors)     // Navigation property
                                                     .Skip(skipCount)
                                                     .Take(pageSize)
                                                     .ToListAsync();


            var viewModel = new BookPaginationViewModel
            {
                Books = paginatedBooks,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalBooks / pageSize)
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            // Retrieve categories and authors from the database
            var categories = await appDBContext.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                })
                .ToListAsync();

            var authors = await appDBContext.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync();

            // Populate ViewBag with categories and authors
            ViewBag.Categories = categories;
            ViewBag.Authors = authors;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBookViewModel addBookViewModel)
        {
            var book = new Book
            {
                Title = addBookViewModel.Title,
                ISBN = addBookViewModel.ISBN,
                PublishedDate = addBookViewModel.PublishedDate,
                CategoriesId = addBookViewModel.CategoriesId,  // Set foreign key
                AuthorsId = addBookViewModel.AuthorId,         // Set foreign key
                IsActive = true
            };

            await appDBContext.Books.AddAsync(book);
            await appDBContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }




        [Route("Book/View/{id}")]
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var Book = await appDBContext.Books.FirstOrDefaultAsync(a => a.Id == id);

            if (Book != null)
            {
                var viewModel = new UpdateBookViewModel
                {
                    Id = Book.Id,
                    Title = Book.Title,
                    ISBN =Book.ISBN,
                    PublishedDate = Book.PublishedDate,
                };
                return View("View", viewModel);
            }

            return RedirectToAction("Index");
        }

        [Route("Book/Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateBookViewModel updateBookViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = await appDBContext.Books.FindAsync(updateBookViewModel.Id);

                if (book != null)
                {
                    book.Title = updateBookViewModel.Title;
                    book.ISBN = updateBookViewModel.ISBN;
                    book.PublishedDate = updateBookViewModel.PublishedDate;

                    await appDBContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            // If model state is invalid, return to the form with errors
            return View("View", updateBookViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateBookViewModel updateBookViewModel)
        {
            var book = await appDBContext.Books.FindAsync(updateBookViewModel.Id);

            if (book != null)
            {
                book.IsActive = false;
                await appDBContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
