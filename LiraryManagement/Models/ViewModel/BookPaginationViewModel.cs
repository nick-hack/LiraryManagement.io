using LiraryManagement.Models.Domain;

namespace LiraryManagement.Models.ViewModel
{
    public class BookPaginationViewModel
    {
        public List<Book> Books { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
