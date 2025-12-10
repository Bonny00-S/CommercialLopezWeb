using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Data;
using ProyectoWebCommercialLopez.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoWebCommercialLopez.Controllers
{
    public class ReportsController : Controller
    {
        private readonly appDbContextCommercial _context;
        private readonly PdfService _pdfService;

        public ReportsController(appDbContextCommercial context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // Vista principal de reportes
        public IActionResult Index()
        {
            return View();
        }

        // Reporte de Ventas por Rango de Fechas (Vista HTML)
        public async Task<IActionResult> SalesReport(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status == "Completada")
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(s => s.SaleDate.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(s => s.SaleDate.Date <= fechaHasta.Value.Date);

            var sales = await query.OrderByDescending(s => s.SaleDate).ToListAsync();

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(sales);
        }

        // Reporte de Ventas en PDF
        public async Task<IActionResult> SalesReportPdf(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Where(s => s.Status == "Completada")
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(s => s.SaleDate.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(s => s.SaleDate.Date <= fechaHasta.Value.Date);

            var sales = await query
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new
                {
                    Numero = s.Id,
                    Fecha = s.SaleDate.ToString("dd/MM/yyyy HH:mm"),
                    Cliente = s.Client!.CompanyName,
                    Total = s.Total,
                    Pago = s.PaymentType,
                    EmitidoPor = s.CreatedByUser!.Person!.Name
                })
                .ToListAsync();

            string periodo = "Todas las ventas completadas";
            if (fechaDesde.HasValue && fechaHasta.HasValue)
                periodo = $"Ventas completadas del {fechaDesde.Value:dd/MM/yyyy} al {fechaHasta.Value:dd/MM/yyyy}";
            else if (fechaDesde.HasValue)
                periodo = $"Ventas completadas desde {fechaDesde.Value:dd/MM/yyyy}";
            else if (fechaHasta.HasValue)
                periodo = $"Ventas completadas hasta {fechaHasta.Value:dd/MM/yyyy}";

            var pdf = _pdfService.CreateReport(
                $"Reporte de Ventas - {periodo}",
                User.Identity?.Name ?? "Usuario",
                sales
            );

            return File(pdf, "application/pdf", $"Reporte_Ventas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        // Reporte de Ventas por Cliente
        public async Task<IActionResult> SalesByClient(int? clientId)
        {
            var query = _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .Where(s => s.Status == "Completada")
                .AsQueryable();

            if (clientId.HasValue && clientId.Value > 0)
                query = query.Where(s => s.ClientId == clientId.Value);

            var sales = await query.OrderByDescending(s => s.SaleDate).ToListAsync();

            ViewBag.Clients = await _context.Client
                .Where(c => c.State == 1)
                .Select(c => new { c.Id, c.CompanyName })
                .ToListAsync();
            ViewBag.ClientId = clientId;

            return View(sales);
        }

        // Reporte de Ventas por Cliente PDF
        public async Task<IActionResult> SalesByClientPdf(int? clientId)
        {
            var query = _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Where(s => s.Status == "Completada")
                .AsQueryable();

            if (clientId.HasValue && clientId.Value > 0)
                query = query.Where(s => s.ClientId == clientId.Value);

            var sales = await query
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new
                {
                    Numero = s.Id,
                    Fecha = s.SaleDate.ToString("dd/MM/yyyy HH:mm"),
                    Cliente = s.Client!.CompanyName,
                    Total = s.Total,
                    Pago = s.PaymentType,
                    EmitidoPor = s.CreatedByUser!.Person!.Name
                })
                .ToListAsync();

            string clientName = "Todos los clientes";
            if (clientId.HasValue && clientId.Value > 0)
            {
                var client = await _context.Client.FindAsync(clientId.Value);
                clientName = client?.CompanyName ?? "Cliente desconocido";
            }

            var pdf = _pdfService.CreateReport(
                $"Reporte de Ventas - {clientName}",
                User.Identity?.Name ?? "Usuario",
                sales
            );

            return File(pdf, "application/pdf", $"Ventas_{clientName}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        // Reporte de Productos Más Vendidos
        public async Task<IActionResult> TopSellingProducts(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.SaleDetail
                .Include(sd => sd.Product)
                .Include(sd => sd.Sale)
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(sd => sd.Sale.SaleDate.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(sd => sd.Sale.SaleDate.Date <= fechaHasta.Value.Date);

            var topProducts = await query
                .GroupBy(sd => new { sd.ProductId, sd.Product!.Description })
                .Select(g => new
                {
                    ProductoId = g.Key.ProductId,
                    Producto = g.Key.Description,
                    CantidadVendida = g.Sum(sd => sd.Quantity),
                    TotalVentas = g.Sum(sd => sd.Subtotal)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(20)
                .ToListAsync();

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(topProducts);
        }

        // Reporte de Productos Más Vendidos en PDF
        public async Task<IActionResult> TopSellingProductsPdf(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.SaleDetail
                .Include(sd => sd.Product)
                .Include(sd => sd.Sale)
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(sd => sd.Sale.SaleDate.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(sd => sd.Sale.SaleDate.Date <= fechaHasta.Value.Date);

            var topProducts = await query
                .GroupBy(sd => new { sd.ProductId, sd.Product!.Description })
                .Select(g => new
                {
                    Producto = g.Key.Description,
                    CantidadVendida = g.Sum(sd => sd.Quantity),
                    TotalVentas = g.Sum(sd => sd.Subtotal)
                })
                .OrderByDescending(x => x.CantidadVendida)
                .Take(20)
                .ToListAsync();

            string periodo = "Histórico";
            if (fechaDesde.HasValue && fechaHasta.HasValue)
                periodo = $"{fechaDesde.Value:dd/MM/yyyy} - {fechaHasta.Value:dd/MM/yyyy}";

            var pdf = _pdfService.CreateReport(
                $"Reporte de Productos Más Vendidos - {periodo}",
                User.Identity?.Name ?? "Usuario",
                topProducts
            );

            return File(pdf, "application/pdf", $"Reporte_ProductosMasVendidos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

        // Reporte de Ingresos por Método de Pago
        public async Task<IActionResult> IncomeByPaymentMethod(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.Sale.AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(s => s.SaleDate.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(s => s.SaleDate.Date <= fechaHasta.Value.Date);

            var incomeByMethod = await query
                .GroupBy(s => s.PaymentType)
                .Select(g => new
                {
                    MetodoPago = g.Key,
                    CantidadVentas = g.Count(),
                    TotalIngresos = g.Sum(s => s.Total)
                })
                .OrderByDescending(x => x.TotalIngresos)
                .ToListAsync();

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(incomeByMethod);
        }
    }
}
