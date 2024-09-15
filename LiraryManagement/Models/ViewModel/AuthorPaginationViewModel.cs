using LiraryManagement.Models.Domain;

namespace LiraryManagement.Models.ViewModel
{
    public class AuthorPaginationViewModel
    {
        public List<Author> Authors { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}
