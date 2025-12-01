using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class PasswocrdResetToken
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(150)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Token is required")]
        [StringLength(200, MinimumLength = 20,
            ErrorMessage = "Token must be between 20 and 200 characters")]
        public string? Token { get; set; }

        [Required]
        public DateTime Expiration { get; set; }
    }
}
