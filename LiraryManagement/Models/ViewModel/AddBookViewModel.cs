using System.ComponentModel.DataAnnotations;

namespace LiraryManagement.Models.ViewModel
{
    public class AddBookViewModel
    {

            [Required(ErrorMessage = "Title is required.")]
            public string Title { get; set; }

            [Required(ErrorMessage = "ISBN is required.")]
            public string ISBN { get; set; }

            [Required(ErrorMessage = "Published date is required.")]
            [DataType(DataType.Date)]
            public DateTime PublishedDate { get; set; }

            [Required(ErrorMessage = "Category is required.")]
            public int CategoriesId { get; set; }

            [Required(ErrorMessage = "Author is required.")]
            public int AuthorId { get; set; }

    }
}
