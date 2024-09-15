using LiraryManagement.Models.Domain;

namespace LiraryManagement.Models.ViewModel
{
    public class CategoryPaginationViewModel
    {
        public List<Category> Categories { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
