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
    public class WareHousesController : Controller
    {

        private readonly appDbContextCommercial _context;

        public WareHousesController(appDbContextCommercial context)
        {
            _context = context;
        }

        // GET: WareHouses
        public async Task<IActionResult> Index()
        {
            return View(await _context.WareHouse.ToListAsync());
        }

        // GET: WareHouses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wareHouse = await _context.WareHouse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wareHouse == null)
            {
                return NotFound();
            }

            return View(wareHouse);
        }

        // GET: WareHouses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WareHouses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatedAt,ModifiedAt,CreatedBy,ModifiedBy")] WareHouse wareHouse)
        {

            if (await _context.WareHouse.AnyAsync(p => p.Name == wareHouse.Name))
            {
                ModelState.AddModelError("Name", " A warehouse with that name already exists and is registered.");
                return View(wareHouse);
            }

            if (ModelState.IsValid)
            {
                wareHouse.CreatedAt = DateTime.Now;
                wareHouse.CreatedBy = int.Parse(User.FindFirst("UserId").Value);
                _context.Add(wareHouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wareHouse);
        }

        // GET: WareHouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wareHouse = await _context.WareHouse.FindAsync(id);
            if (wareHouse == null)
            {
                return NotFound();
            }
            return View(wareHouse);
        }

        // POST: WareHouses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CreatedAt,ModifiedAt,CreatedBy,ModifiedBy")] WareHouse wareHouse)
        {
            if (id != wareHouse.Id)
                return NotFound();

            // 🔍 Obtener el registro ORIGINAL desde la BD
            var original = await _context.WareHouse.AsNoTracking()
                                                   .FirstOrDefaultAsync(w => w.Id == id);

            if (original == null)
                return NotFound();

            // ⭐ SI el nombre fue modificado recién validamos duplicados
            if (original.Name != wareHouse.Name)
            {
                // 🔥 Validar que NO exista otro con el mismo nombre
                bool nameExists = await _context.WareHouse
                    .AnyAsync(w => w.Name == wareHouse.Name && w.Id != wareHouse.Id);

                if (nameExists)
                {
                    ModelState.AddModelError("Name", "A warehouse with that name already exists and is registered.");
                    return View(wareHouse);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    wareHouse.ModifiedAt = DateTime.Now;
                    wareHouse.ModifiedBy = int.Parse(User.FindFirst("UserId").Value);

                    _context.Update(wareHouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WareHouseExists(wareHouse.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(wareHouse);
        }


        // GET: WareHouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wareHouse = await _context.WareHouse
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wareHouse == null)
            {
                return NotFound();
            }

            return View(wareHouse);
        }

        // POST: WareHouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wareHouse = await _context.WareHouse.FindAsync(id);
            if (wareHouse != null)
            {
                _context.WareHouse.Remove(wareHouse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WareHouseExists(int id)
        {
            return _context.WareHouse.Any(e => e.Id == id);
        }
    }
}
