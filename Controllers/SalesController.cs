using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Data;
using ProyectoWebCommercialLopez.Models;

namespace ProyectoWebCommercialLopez.Controllers
{
    public class SalesController : Controller
    {
        private readonly appDbContextCommercial _context;

        public SalesController(appDbContextCommercial context)
        {
            _context = context;
        }

        // GET: Sales
        public async Task<IActionResult> Index(string? fechaDesde, string? fechaHasta, int? clienteId, string? estado)
        {
            var salesQuery = _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(fechaDesde) && DateTime.TryParse(fechaDesde, out DateTime desde))
            {
                salesQuery = salesQuery.Where(s => s.SaleDate.Date >= desde.Date);
                ViewBag.FechaDesde = fechaDesde;
            }

            if (!string.IsNullOrEmpty(fechaHasta) && DateTime.TryParse(fechaHasta, out DateTime hasta))
            {
                salesQuery = salesQuery.Where(s => s.SaleDate.Date <= hasta.Date);
                ViewBag.FechaHasta = fechaHasta;
            }

            if (clienteId.HasValue && clienteId > 0)
            {
                salesQuery = salesQuery.Where(s => s.ClientId == clienteId);
                ViewBag.ClienteId = clienteId.ToString();
            }

            if (!string.IsNullOrEmpty(estado))
            {
                salesQuery = salesQuery.Where(s => s.Status == estado);
                ViewBag.Estado = estado;
            }

            var sales = await salesQuery
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();

            // Cargar clientes para el filtro
            ViewBag.Clients = await _context.Client.Select(c => new { c.Id, c.CompanyName }).ToListAsync();

            return View(sales);
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.ClientId = new SelectList(_context.Client, "Id", "CompanyName");
            
            var products = _context.Product
                .Where(p => p.Stock > 0)
                .Select(p => new { p.Id, p.Description, p.Price, p.Stock })
                .ToList();
            
            ViewBag.Products = products;
            
            // Obtener información del usuario logueado
            int userLogged = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var user = await _context.User
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.IdPerson == userLogged);
            
            ViewBag.UserName = user?.Person?.Name ?? "Usuario";
            
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                int userLogged = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                // Crear la venta
                var sale = new Sale
                {
                    SaleDate = DateTime.Now,
                    ClientId = model.ClientId,
                    Discount = model.Discount,
                    Total = model.Total,
                    PaymentType = model.PaymentType,
                    Status = "Completada",
                    CreatedAt = DateTime.Now,
                    CreatedBy = userLogged
                };

                _context.Sale.Add(sale);
                await _context.SaveChangesAsync();

                // Crear los detalles de venta
                if (model.Details != null)
                {
                    foreach (var detail in model.Details)
                {
                    var product = await _context.Product.FindAsync(detail.ProductId);
                    
                    if (product == null || product.Stock < detail.Quantity)
                    {
                        return BadRequest(new { message = $"Stock insuficiente para el producto {product?.Description}" });
                    }

                    var saleDetail = new SaleDetail
                    {
                        SaleId = sale.Id,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        Subtotal = detail.Subtotal,
                        CreatedAt = DateTime.Now
                    };

                    _context.SaleDetail.Add(saleDetail);

                    // Actualizar stock del producto
                    product.Stock -= (short)detail.Quantity;
                    product.ModifiedAt = DateTime.Now;
                    product.ModifiedBy = userLogged;
                    _context.Update(product);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Venta registrada exitosamente", saleId = sale.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al registrar la venta: {ex.Message}" });
            }
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Include(s => s.SaleDetails)
                .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sales/PrintReceipt/5
        public async Task<IActionResult> PrintReceipt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .Include(s => s.Client)
                .Include(s => s.CreatedByUser!)
                    .ThenInclude(u => u.Person!)
                .Include(s => s.SaleDetails)
                    .ThenInclude(sd => sd.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sale
                .Include(s => s.SaleDetails)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale != null)
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");

                // Marcar como anulado en lugar de eliminar
                sale.Status = "Anulado";
                sale.ModifiedAt = DateTime.Now;
                sale.ModifiedBy = userId;

                // Restaurar stock de productos
                if (sale.SaleDetails != null)
                {
                    foreach (var detail in sale.SaleDetails)
                    {
                        var product = await _context.Product.FindAsync(detail.ProductId);
                        if (product != null)
                        {
                            product.Stock += (short)detail.Quantity;
                            product.ModifiedAt = DateTime.Now;
                            product.ModifiedBy = userId;
                            _context.Update(product);
                        }
                    }
                }
                
                _context.Update(sale);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sale.Any(e => e.Id == id);
        }
    }

    // Modelo para recibir datos de venta
    public class SaleCreateModel
    {
        public int ClientId { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string? PaymentType { get; set; }
        public List<SaleDetailModel>? Details { get; set; }
    }

    public class SaleDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
