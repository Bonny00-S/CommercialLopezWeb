using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CI/NIT is required")]
        [StringLength(15, MinimumLength = 6,
            ErrorMessage = "CI/NIT must be between 6 and 15 characters")]
        [RegularExpression(@"^[1-9][0-9A-Za-z-]*$", 
            ErrorMessage = "CI/NIT cannot start with zero and can only contain numbers, letters and hyphens")]
        [Display(Name = "CI/NIT")]
        public string? DocumentNumber { get; set; }

        [Required(ErrorMessage = "Reason Social is required")]
        [StringLength(150, MinimumLength = 3,
            ErrorMessage = "Reason Social must be between 3 and 150 characters")]
        [Display(Name = "Reason Social")]
        public string? CompanyName { get; set; }

        [RegularExpression(@"^[0-9+\-\s()]+$",
            ErrorMessage = "Invalid phone format. Only numbers, spaces and symbols +()-")]
        [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }


        [RegularExpression(@"^[A-Za-z0-9ÁÉÍÓÚáéíóúÑñ #.,\-\/º°()]+$",
            ErrorMessage = "Address contains invalid characters.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 200 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "State is required")]
        [Display(Name = "State")]
        public byte State { get; set; } // 1 = Activo, 0 = Inactivo

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        // Navegación
        public ICollection<Sale>? Sales { get; set; }
    }
}
