using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoWebCommercialLopez.Models
{
    public class SaleDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Venta")]
        [ForeignKey("Sale")]
        public int SaleId { get; set; }

        [Required]
        [Display(Name = "Producto")]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Precio Unitario")]
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

       
      
        [Display(Name = "Subtotal")]
        [Precision(18, 2)]
        public decimal Subtotal { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        // Navegación
        public Sale? Sale { get; set; }
        public Product? Product { get; set; }
    }
}
