using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Supplier
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The reason social is mandatory.")]
        [StringLength(150, ErrorMessage = "The reason social it cannot exceed 150 characters.")]
        public string? RazonSocial { get; set; }

        [Required(ErrorMessage = "The NIT is mandatory.")]
        [StringLength(20, ErrorMessage = "The NIT cannot exceed 20 characters.")]
        public string? NIT { get; set; }

        [StringLength(20, ErrorMessage = "The phone number cannot exceed 20 characters.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "You must enter a valid email address.")]
        [StringLength(120, ErrorMessage = "The email cannot exceed 120 characters.")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "The address cannot exceed 200 characters.")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "The city cannot exceed 100 characters.")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "The country name cannot exceed 100 characters.")]
        public string? Country { get; set; }

        [Range(0, 1, ErrorMessage = "The state must be 0 (Inactive) or 1 (Active).")]
        public int State { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }


        public ICollection<Product>? Products { get; set; }
    }
}