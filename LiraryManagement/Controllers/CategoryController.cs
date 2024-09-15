using LiraryManagement.Data;
using LiraryManagement.Models.Domain;
using LiraryManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LiraryManagement.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBContext appDBContext;

        public CategoryController(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            // Total number of Categories
            int totalCategories = await appDBContext.Categories.CountAsync();

            // Calculate the number of records to skip
            int skipCount = (page - 1) * pageSize;

            // Get paginated Categories
            var paginatedCategories = await appDBContext.Categories
                                          .Where(a => a.IsActive)   // Add this line to filter active authors
                                          .Skip(skipCount)
                                          .Take(pageSize)
                                          .ToListAsync();


            // Create a view model to send both data and pagination info
            var viewModel = new CategoryPaginationViewModel
            {
                Categories = paginatedCategories,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalCategories / pageSize)
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddCategoryViewModel addCategoryViewModel)
        {
            var Category = new Category()
            {
                CategoryName = addCategoryViewModel.CategoryName,
                IsActive = true,

            };
            await appDBContext.Categories.AddAsync(Category);
            await appDBContext.SaveChangesAsync();
            return RedirectToAction("Index");

        }

        [Route("Categories/View/{id}")]
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var category = await appDBContext.Categories.FirstOrDefaultAsync(a => a.Id == id);

            if (category != null)
            {
                var viewModel = new UpdateCategoryViewModel
                {
                    Id = id,
                    CategoryName=category.CategoryName,
                    IsActive = category.IsActive,
                };
                return View("View", viewModel);
            }

            return RedirectToAction("Index");
        }

        [Route("Categories/Edit")]
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryViewModel updateCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                var book = await appDBContext.Categories.FindAsync(updateCategoryViewModel.Id);

                if (book != null)
                {
                   book.Id = updateCategoryViewModel.Id;
                   book.CategoryName = updateCategoryViewModel.CategoryName;

                    await appDBContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            // If model state is invalid, return to the form with errors
            return View("View", updateCategoryViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateCategoryViewModel updateCategoryViewModel)
        {
            var book = await appDBContext.Categories.FindAsync(updateCategoryViewModel.Id);

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
