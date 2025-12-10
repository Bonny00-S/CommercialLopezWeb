using System.Collections.Generic;

namespace ProyectoWebCommercialLopez.Models.ViewModels
{
    public class SaleCreateVM
    {
        public int ClientId { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string? PaymentType { get; set; }
        public List<SaleDetailVM>? Details { get; set; }
    }

    public class SaleDetailVM
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
