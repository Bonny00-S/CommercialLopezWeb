using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Data;
using ProyectoWebCommercialLopez.Models;
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

        private readonly IWebHostEnvironment _env;

        public ProductsController(appDbContextCommercial context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            var appDbContextCommercial = _context.Product.Include(p => p.Category).Include(p => p.Supplier).Include(p => p.Warehouse);
            return View(await appDbContextCommercial.ToListAsync());
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

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Supplier, "Id", "RazonSocial");
            ViewData["WarehouseId"] = new SelectList(_context.WareHouse, "Id", "Name");
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
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
