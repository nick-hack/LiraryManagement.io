using System.ComponentModel.DataAnnotations;

namespace LiraryManagement.Models.ViewModel
{
    public class AddAuthorViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "DateofBirth is required.")]
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }
}
