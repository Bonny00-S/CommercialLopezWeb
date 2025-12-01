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
            if (ModelState.IsValid)
            {
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
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wareHouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WareHouseExists(wareHouse.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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
