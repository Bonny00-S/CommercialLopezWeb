using System.ComponentModel.DataAnnotations;

namespace ProyectoWebCommercialLopez.Models
{
    public class WareHouse
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del almacén es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string? Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

      
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}