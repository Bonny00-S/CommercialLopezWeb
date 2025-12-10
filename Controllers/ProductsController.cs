using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Data;
using ProyectoWebCommercialLopez.Models;
using ProyectoWebCommercialLopez.Models.ViewModels;
using ProyectoWebCommercialLopez.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoWebCommercialLopez.Controllers
{
    [Authorize(Roles = "Admin,Almacen")]
    public class ProductsController : Controller
    {
        private readonly appDbContextCommercial _context;
        private readonly PdfService _pdfService;

        private readonly IWebHostEnvironment _env;

        public ProductsController(appDbContextCommercial context, IWebHostEnvironment env, PdfService pdfService)
        {
            _context = context;
            _env = env;
            _pdfService = pdfService;
        }


        public async Task<IActionResult> Index(
            int? categoryId,
            int? supplierId,
            int? warehouseId,
            string search,
            string sort)
        {
            var query = _context.Product
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .AsQueryable();

            // 🔍 FILTROS
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (supplierId.HasValue)
                query = query.Where(p => p.SupplierId == supplierId.Value);

            if (warehouseId.HasValue)
                query = query.Where(p => p.WarehouseId == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Description.Contains(search));

            // 🔽 ORDENAMIENTO
            query = sort switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),

                "stock_asc" => query.OrderBy(p => p.Stock),
                "stock_desc" => query.OrderByDescending(p => p.Stock),

                "desc_asc" => query.OrderBy(p => p.Description),
                "desc_desc" => query.OrderByDescending(p => p.Description),

                _ => query.OrderBy(p => p.Id)
            };

            var productos = await query.ToListAsync();

            // Filtros (listas)
            ViewBag.Categories = await _context.Category.ToListAsync();
            ViewBag.Suppliers = await _context.Supplier.ToListAsync();
            ViewBag.Warehouses = await _context.WareHouse.ToListAsync();

            // 🔥 Stock bajo
            ViewBag.LowStockCount = productos.Count(p => p.Stock < 10);

            return View(productos);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            List<string> missing = new List<string>();

            if (!_context.Category.Any())
                missing.Add("categories");

            if (!_context.Supplier.Any())
                missing.Add("suppliers");

            if (!_context.WareHouse.Any())
                missing.Add("warehouses");

            // Si faltan uno o más…
            if (missing.Count > 0)
            {
                ViewBag.BlockCreate = true;

                // Mensaje correcto según cantidad
                if (missing.Count == 1)
                    ViewBag.BlockMessage = $"You must register {missing[0]} before creating a product.";
                else if (missing.Count == 2)
                    ViewBag.BlockMessage = $"You must register both {missing[0]} and {missing[1]} before creating a product.";
                else
                    ViewBag.BlockMessage = "You must register categories, suppliers, and warehouses before creating a product.";

                return View();
            }

            // Si no falta nada → carga normal
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "RazonSocial");
            ViewData["WarehouseId"] = new SelectList(_context.WareHouse, "Id", "Name");

            ViewBag.BlockCreate = false;

            return View();
        }



        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
                ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "RazonSocial", product.SupplierId);
                ViewData["WarehouseId"] = new SelectList(_context.WareHouse, "Id", "Name", product.WarehouseId);
                return View(product);
            }

            if (await _context.Product.AnyAsync(p => p.Description == product.Description))
            {
                ModelState.AddModelError("Description", " ya existe un producto con ese nombre está registrado.");
                return View(product);
            }


            // 📌 1. Subir imagen si existe
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Carpeta destino
                string folder = Path.Combine(_env.WebRootPath, "images/products");

                // Crear carpeta si no existe
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // Nombre único (GUID)
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

                // Ruta completa
                string filePath = Path.Combine(folder, fileName);

                // Guardar archivo físico
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Guardar nombre en la BD
                product.Image = fileName;
            }

            // 📌 2. Auditoría
            product.CreatedAt = DateTime.Now;
            product.CreatedBy = int.Parse(User.FindFirst("UserId").Value);

            // 📌 3. Guardar en BD
            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "NIT", product.SupplierId);
            ViewData["WarehouseId"] = new SelectList(_context.WareHouse, "Id", "Name", product.WarehouseId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile ImageFile)
        {
            if (id != product.Id)
                return NotFound();

            // 🔹 Obtener el producto original de la BD
            var productDb = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);

            if (productDb == null)
                return NotFound();

            // 🔥 Actualizar solo los campos editables
            productDb.Description = product.Description;
            productDb.Price = product.Price;
            productDb.Stock = product.Stock;
            productDb.CategoryId = product.CategoryId;
            productDb.WarehouseId = product.WarehouseId;
            productDb.SupplierId = product.SupplierId;

            // 🔥 Auditoría
            productDb.ModifiedAt = DateTime.Now;
            productDb.ModifiedBy = int.Parse(User.FindFirst("UserId").Value);

            // -------------------------------
            //   📌 MANEJO DE IMAGEN
            // -------------------------------
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // 1️⃣ BORRAR imagen anterior si existía
                if (!string.IsNullOrEmpty(productDb.Image))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", productDb.Image);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // 2️⃣ GUARDAR nueva imagen
                var newFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", newFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                productDb.Image = newFileName; // Guardar nuevo nombre
            }
            // 📌 Si ImageFile es null → NO se toca productDb.Image → mantiene la anterior.

            // Guardar cambios
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);

            if (product != null)
            {
         
                if (!string.IsNullOrEmpty(product.Image))
                {
                    string folder = Path.Combine(_env.WebRootPath, "images/products");
                    string filePath = Path.Combine(folder, product.Image);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); 
                    }
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        public IActionResult ReportsIndex()
        {
            return View();
        }

        public async Task<IActionResult> ReportProducts()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> ReportProductsPdf()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Select(p => new
                {
                    p.Description,
                    p.Price,
                    p.Stock
                })
                .ToListAsync();

            var pdf = _pdfService.CreateReport(
                "Reporte de Productos",
                User.Identity.Name,
                products
            );

            // 📌 Crear nombre dinámico usando fecha y hora actual
            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string fileName = $"Reporte_Productos_{date}.pdf";

            return File(pdf, "application/pdf", fileName);
        }



        public async Task<IActionResult> ReportLowStock()
        {
            var lowStock = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Stock < 10)
                .ToListAsync();

            return View(lowStock);
        }

        public async Task<IActionResult> ReportLowStockPdf()
        {
            var lowStock = await _context.Product
                .Include(p => p.Category)
                .Where(p => p.Stock < 10)
                .Select(p => new
                {
                    p.Description,
                    Category = p.Category.Name,
                    p.Price,
                    p.Stock
                })
                .ToListAsync();

            var pdf = _pdfService.CreateReport(
                "Productos con Stock Bajo",
                User.Identity.Name,
                lowStock
            );

            // 📌 Crear nombre dinámico con fecha y hora
            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string fileName = $"Reporte_LowStock_{date}.pdf";

            return File(pdf, "application/pdf", fileName);
        }


        public async Task<IActionResult> ReportByCategory()
        {
            var data = await _context.Category
                .Select(c => new CategoryReportVM
                {
                    CategoryName = c.Name,
                    TotalProductos = c.Products.Count(),
                    TotalStock = c.Products.Sum(p => p.Stock),
                    ValorTotal = c.Products.Sum(p => p.Stock * p.Price)
                })
                .ToListAsync();

            return View(data);
        }

        public async Task<IActionResult> ReportByCategoryPdf()
        {
            var data = await _context.Category
                .Select(c => new CategoryReportVM
                {
                    CategoryName = c.Name,
                    TotalProductos = c.Products.Count(),
                    TotalStock = c.Products.Sum(p => p.Stock),
                    ValorTotal = c.Products.Sum(p => p.Stock * p.Price)
                })
                .ToListAsync();

            var pdf = _pdfService.CreateReport(
                "Reporte por Categoría",
                User.Identity.Name,
                data
            );

            // 📌 Crear nombre dinámico con fecha y hora
            string date = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            string fileName = $"Reporte_Categorias_{date}.pdf";

            return File(pdf, "application/pdf", fileName);
        }



        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
