namespace ProyectoWebCommercialLopez.Models.ViewModels
{
    public class CategoryReportVM
    {
        public string CategoryName { get; set; }
        public int TotalProductos { get; set; }
        public int TotalStock { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
