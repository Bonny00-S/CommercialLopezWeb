using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoWebCommercialLopez.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Venta")]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; }

        [Required]
        [Display(Name = "Cliente")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Display(Name = "Descuento")]
        [Precision(18, 2)]
        public decimal Discount { get; set; }

        [Display(Name = "Total")]
        [Precision(18, 2)]
        public decimal Total { get; set; }

        [Required]
        [Display(Name = "Tipo de Pago")]
        [StringLength(50)]
        public string? PaymentType { get; set; }

        [Display(Name = "Estado")]
        [StringLength(50)]
        public string? Status { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        // Navegación
        public Client? Client { get; set; }
        
        [ForeignKey("CreatedBy")]
        public User? CreatedByUser { get; set; }
        
        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}
