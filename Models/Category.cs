using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "Name must be between 3 and 100 characters")]
        public string? Name { get; set; }

        [StringLength(250, MinimumLength = 5,
            ErrorMessage = "Description must be between 5 and 250 characters")]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
