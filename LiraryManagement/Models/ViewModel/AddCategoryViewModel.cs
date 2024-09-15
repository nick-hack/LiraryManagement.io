using System.ComponentModel.DataAnnotations;

namespace LiraryManagement.Models.ViewModel
{
    public class AddCategoryViewModel
    {
        [Required(ErrorMessage = "CategoryName is required.")]
        public string CategoryName { get; set; }
    }
}
