using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoWebCommercialLopez.Models
{
    public class Product
    {

        [Key]
        public int Id { get; set; }

        public string? Description { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public short Stock { get; set; }

        public string? Image { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }


        [ForeignKey("Category")]
        [Display(Name ="Category")]
        public int CategoryId { get; set; }

        [ForeignKey("WareHouse")]
        [Display(Name = "WareHouse")]
        public int WarehouseId { get; set; }

        [ForeignKey("Supplier")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }


        public Category? Category { get; set; }
        public WareHouse? Warehouse { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
