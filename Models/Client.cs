using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "CI/NIT es requerido")]
        [StringLength(15, MinimumLength = 5,
            ErrorMessage = "CI/NIT debe tener entre 5 y 15 caracteres")]
        [Display(Name = "CI/NIT")]
        public string? DocumentNumber { get; set; }

        [Required(ErrorMessage = "Razón Social es requerida")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Reason Social debe tener entre 2 y 200 caracteres")]
        [Display(Name = "Reason Social")]
        public string? CompanyName { get; set; }

        [Phone(ErrorMessage = "Formato de Phone inválido")]
        [StringLength(20)]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }


        [StringLength(200)]
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
