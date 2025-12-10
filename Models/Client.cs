using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CI/NIT es requerido")]
        [StringLength(15, MinimumLength = 6,
            ErrorMessage = "CI/NIT debe tener entre 6 y 15 caracteres")]
        [RegularExpression(@"^[1-9][0-9A-Za-z-]*$", 
            ErrorMessage = "CI/NIT no puede empezar con cero y solo puede contener números, letras y guiones")]
        [Display(Name = "CI/NIT")]
        public string? DocumentNumber { get; set; }

        [Required(ErrorMessage = "Razón Social es requerida")]
        [StringLength(150, MinimumLength = 3,
            ErrorMessage = "Razón Social debe tener entre 3 y 150 caracteres")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9&.,\s-]+$",
            ErrorMessage = "Razón Social contiene caracteres no válidos")]
        [Display(Name = "Razón Social")]
        public string? CompanyName { get; set; }

        [RegularExpression(@"^[0-9+\-\s()]+$",
            ErrorMessage = "Formato de teléfono inválido. Solo números, espacios y símbolos +()-")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }


        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        [Display(Name = "Dirección")]
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
