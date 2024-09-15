using LiraryManagement.Models.ViewModel;
using LiraryManagement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using LiraryManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace LiraryManagement.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AppDBContext appDBContext;

        public AuthorController(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // Total number of authors
            int totalAuthors = await appDBContext.Authors.CountAsync();

            // Calculate the number of records to skip
            int skipCount = (page - 1) * pageSize;

            // Get paginated authors and count associated books for each author
            var paginatedAuthors = await appDBContext.Authors
                                          .Where(a => a.IsActive)   // Filter only active authors
                                          .Skip(skipCount)
                                          .Take(pageSize)
                                          .Select(author => new
                                          {
                                              Author = author,
                                              BookCount = appDBContext.Books.Count(b => b.Authors.Id == author.Id)  // Count books for each author
                                          })
                                          .ToListAsync();

            // Create a view model to send both data and pagination info
            var viewModel = new AuthorPaginationViewModel
            {
                // Map only the authors from the query result
                Authors = paginatedAuthors.Select(a => a.Author).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalAuthors / pageSize)
            };

            // If you want to pass the author and book count to the view, modify the view model to include book counts.
            ViewBag.AuthorBookCounts = paginatedAuthors.ToDictionary(a => a.Author.Id, a => a.BookCount);

            return View(viewModel);
        }



        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddAuthorViewModel addAuthorViewModel)
        {
            // Check if the author name already exists in the database
            var existingAuthor = await appDBContext.Authors
                                      .FirstOrDefaultAsync(a => a.Name == addAuthorViewModel.Name);

            if (existingAuthor != null)
            {
                // Name already exists, add a model error
                ModelState.AddModelError("Name", "An author with this name already exists.");

                // Return the view with the current data to show the validation message
                return View(addAuthorViewModel);
            }

            // If no duplicate, create a new author
            var author = new Author()
            {
                Name = addAuthorViewModel.Name,
                DateOfBirth = addAuthorViewModel.DateOfBirth,
                IsActive = true
            };

            await appDBContext.Authors.AddAsync(author);
            await appDBContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [Route("Authors/View/{id}")]
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var author = await appDBContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (author != null)
            {
                var viewModel = new UpdateAuthorViewModel
                {
                    Id = author.Id,
                    Name = author.Name,
                    DateOfBirth = author.DateOfBirth,
                    IsActive = author.IsActive
                };
                return View("View", viewModel);
            }

            return RedirectToAction("Index");
        }

        [Route("Authors/Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateAuthorViewModel updateAuthorViewModel)
        {
            if (ModelState.IsValid)
            {
                var author = await appDBContext.Authors.FindAsync(updateAuthorViewModel.Id);

                if (author != null)
                {
                    author.Name = updateAuthorViewModel.Name;
                    author.DateOfBirth = updateAuthorViewModel.DateOfBirth;
                    author.IsActive = updateAuthorViewModel.IsActive;

                    await appDBContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            // If model state is invalid, return to the form with errors
            return View("View", updateAuthorViewModel);
        }

        [HttpPost]
        public async Task<IActionResult>Delete(UpdateAuthorViewModel updateAuthorViewModel)
        {
            var book = await appDBContext.Authors.FindAsync(updateAuthorViewModel.Id);

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
