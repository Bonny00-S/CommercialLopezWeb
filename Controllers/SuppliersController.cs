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
    public class SuppliersController : Controller
    {
        private readonly appDbContextCommercial _context;

        public SuppliersController(appDbContextCommercial context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Supplier.ToListAsync());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RazonSocial,NIT,Phone,Email,Address,City,Country,State,CreatedAt,ModifiedAt,CreatedBy,ModifiedBy")] Supplier supplier)
        {
            int userLogged = int.Parse(User.FindFirst("UserId").Value);

            // Normalizar datos
            supplier.RazonSocial = supplier.RazonSocial?.Trim().ToUpper();
            supplier.Email = supplier.Email?.Trim().ToLower();
            supplier.NIT = supplier.NIT?.Trim();

            supplier.State = 1;
            supplier.CreatedAt = DateTime.Now;
            supplier.CreatedBy = userLogged;

            // ============================
            // 🔥 VALIDACIONES DE DUPLICADOS
            // ============================

            // RAZON SOCIAL
            bool rsExists = await _context.Supplier
                .AnyAsync(s => s.RazonSocial.ToUpper() == supplier.RazonSocial);

            if (rsExists)
                ModelState.AddModelError("RazonSocial", "A supplier with this business name already exists.");

            // NIT
            bool nitExists = await _context.Supplier
                .AnyAsync(s => s.NIT == supplier.NIT);

            if (nitExists)
                ModelState.AddModelError("NIT", "This NIT is already registered.");

            // EMAIL
            bool emailExists = await _context.Supplier
                .AnyAsync(s => s.Email.ToLower() == supplier.Email.ToLower());

            if (emailExists)
                ModelState.AddModelError("Email", "This email is already registered.");

            // ============================
            // 🔥 SI FALLA VALIDACIÓN
            // ============================
            if (!ModelState.IsValid)
                return View(supplier);

            // ============================
            // ✔ GUARDAR EN BD
            // ============================
            _context.Add(supplier);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RazonSocial,NIT,Phone,Email,Address,City,Country,State,CreatedAt,ModifiedAt,CreatedBy,ModifiedBy")] Supplier supplier)
        {
            if (id != supplier.Id)
                return NotFound();

            int userLogged = int.Parse(User.FindFirst("UserId").Value);

            // Normalizar campos
            supplier.RazonSocial = supplier.RazonSocial?.Trim().ToUpper();
            supplier.Email = supplier.Email?.Trim().ToLower();
            supplier.NIT = supplier.NIT?.Trim();

            supplier.State = 1;
            supplier.ModifiedAt = DateTime.Now;
            supplier.ModifiedBy = userLogged;

            // ============================
            // 🔥 VALIDACIONES DE DUPLICADOS
            // ============================

            // Razon Social duplicada (excepto el mismo)
            bool rsExists = await _context.Supplier
                .AnyAsync(s => s.RazonSocial.ToUpper() == supplier.RazonSocial && s.Id != supplier.Id);

            if (rsExists)
                ModelState.AddModelError("RazonSocial", "Another supplier with this business name already exists.");

            // NIT duplicado
            bool nitExists = await _context.Supplier
                .AnyAsync(s => s.NIT == supplier.NIT && s.Id != supplier.Id);

            if (nitExists)
                ModelState.AddModelError("NIT", "This NIT is already registered by another supplier.");

            // Email duplicado
            bool emailExists = await _context.Supplier
                .AnyAsync(s => s.Email.ToLower() == supplier.Email.ToLower() && s.Id != supplier.Id);

            if (emailExists)
                ModelState.AddModelError("Email", "This email is already registered by another supplier.");

            // ============================
            // 🔥 SI HAY ERRORES → NO GUARDAR
            // ============================
            if (!ModelState.IsValid)
                return View(supplier);

            // ============================
            // ✔ ACTUALIZAR EN BD
            // ============================
            try
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(supplier.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier != null)
            {
                _context.Supplier.Remove(supplier);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
