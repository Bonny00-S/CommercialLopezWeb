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
        [Display(Name = "Sale Date")]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; }

        [Required]
        [Display(Name = "Client")]
        [ForeignKey("Client")]
        public int ClientId { get; set; }


        [Display(Name = "Discount")]
        [Precision(18, 2)]
        public decimal Discount { get; set; }

        [Display(Name = "Total")]
        [Precision(18, 2)]
        public decimal Total { get; set; }

        [Display(Name = "Payment Type")]
        [StringLength(50)]
        public string? PaymentType { get; set; } // Efectivo, Tarjeta, QR, etc.

        [Display(Name = "Status")]
        [StringLength(50)]
        public string? Status { get; set; } // Completada, Cancelada, Pendiente


        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ModifiedAt { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }

        // Navegación
        public Client? Client { get; set; }
        public ICollection<SaleDetail>? SaleDetails { get; set; }
    }
}
