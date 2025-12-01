using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class Supplier
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(150, ErrorMessage = "La razón social no puede superar los 150 caracteres.")]
        public string? RazonSocial { get; set; }

        [Required(ErrorMessage = "El NIT es obligatorio.")]
        [StringLength(20, ErrorMessage = "El NIT no puede superar los 20 caracteres.")]
        public string? NIT { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [StringLength(120, ErrorMessage = "El email no puede superar los 120 caracteres.")]
        public string? Email { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede superar los 200 caracteres.")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "La ciudad no puede superar los 100 caracteres.")]
        public string? City { get; set; }

        [StringLength(100, ErrorMessage = "El país no puede superar los 100 caracteres.")]
        public string? Country { get; set; }

        [Range(0, 1, ErrorMessage = "El estado debe ser 0 (Inactivo) o 1 (Activo).")]
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