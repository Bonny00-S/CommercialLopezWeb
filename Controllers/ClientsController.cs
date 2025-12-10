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
    public class ClientsController : Controller
    {
        private readonly appDbContextCommercial _context;

        public ClientsController(appDbContextCommercial context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DocumentNumber,CompanyName,Phone,Address,State,CreatedAt,ModifiedAt,CreatedBy,ModifiedBy")] Client client)
        {
            // ============================================
            // VALIDACIONES DE SEGURIDAD Y FORMATO CI/NIT
            // ============================================
            if (!string.IsNullOrWhiteSpace(client.DocumentNumber))
            {
                var docNumber = client.DocumentNumber.Trim();

                // Validar caracteres peligrosos
                if (docNumber.Any(c => "<>'\"\\/ \\;=()".Contains(c)))
                {
                    ModelState.AddModelError("DocumentNumber", "The CI/NIT contains prohibited characters");
                }

                // No puede empezar con cero
                if (docNumber.StartsWith("0"))
                {
                    ModelState.AddModelError("DocumentNumber", "The CI/NIT cannot start with zero");
                }

                // Verificar duplicados
                var existingClient = await _context.Client
                    .FirstOrDefaultAsync(c => c.DocumentNumber == docNumber);
                
                if (existingClient != null)
                {
                    ModelState.AddModelError("DocumentNumber", "There is already a registered customer with this CI/NIT");
                }
            }

            // ============================================
            // VALIDACIONES DE SEGURIDAD Y FORMATO REASON SOCIAL
            // ============================================
            // Validaciones de Reason Social
            if (!string.IsNullOrWhiteSpace(client.CompanyName))
            {
                var companyName = client.CompanyName.Trim();

                // Validar longitud
                if (companyName.Length < 2 || companyName.Length > 100)
                {
                    ModelState.AddModelError("CompanyName", "The full name must be between 2 and 100 characters.");
                }

                // Validar que no tenga múltiples espacios seguidos
                if (companyName.Contains("  "))
                {
                    ModelState.AddModelError("CompanyName", "Multiple spaces in a row are not allowed.");
                }

                // Validar solo letras, espacios y acentos
                if (!System.Text.RegularExpressions.Regex.IsMatch(companyName, @"^[A-Za-zÁÉÍÓÚáéíóúÑñ ]+$"))
                {
                    ModelState.AddModelError("CompanyName", "The name can only contain letters and spaces.");
                }
            }
            else
            {
                ModelState.AddModelError("CompanyName", "The Reason Social is mandatory..");
            }

            // Validar formato de teléfono si se proporciona
            if (!string.IsNullOrWhiteSpace(client.Phone))
            {
                var phoneDigits = new string(client.Phone.Where(char.IsDigit).ToArray());
                if (phoneDigits.Length < 7 || phoneDigits.Length > 15)
                {
                    ModelState.AddModelError("Phone", "The phone number must be between 7 and 15 digits.");
                }
            }

            int userLogged = int.Parse(User.FindFirst("UserId").Value);
            if (ModelState.IsValid)
            {
                // Normalizar y sanitizar datos antes de guardar
                client.DocumentNumber = client.DocumentNumber?.Trim().ToUpper();
                client.CompanyName = client.CompanyName?.Trim();
                client.Phone = client.Phone?.Trim();
                client.Address = client.Address?.Trim();

                client.CreatedAt = DateTime.Now;
                client.State = 1; // Activo por defecto
                client.CreatedBy = userLogged;
                
                _context.Add(client);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Customer successfully registered";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocumentNumber,CompanyName,Phone,Address,State,CreatedAt,ModifiedAt,CreatedBy,ModifiedBy")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int userLogged = int.Parse(User.FindFirst("UserId").Value);
                    client.ModifiedAt = DateTime.Now;
                    client.ModifiedBy = userLogged; // Aquí deberías asignar el ID del usuario actual

                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }

        // GET: Clients/SearchClients
        [HttpGet]
        public async Task<IActionResult> SearchClients(string search)
        {
            if (string.IsNullOrEmpty(search) || search.Length < 2)
            {
                return Ok(new List<object>());
            }

            var clients = await _context.Client
                .Where(c => c.State == 1 && 
                    (c.DocumentNumber.ToUpper().Contains(search.ToUpper()) || 
                     c.CompanyName.ToUpper().Contains(search.ToUpper())))
                .Take(10)
                .Select(c => new
                {
                    id = c.Id,
                    companyName = c.CompanyName,
                    documentNumber = c.DocumentNumber,
                    phone = c.Phone,
                    address = c.Address
                })
                .ToListAsync();

            return Ok(clients);
        }

        // GET: Clients/SearchByDocument
        [HttpGet]
        public async Task<IActionResult> SearchByDocument(string document)
        {
            if (string.IsNullOrEmpty(document))
            {
                return BadRequest(new { message = "You must provide a document number" });
            }

            var client = await _context.Client
                .Where(c => c.DocumentNumber == document.ToUpper() && c.State == 1)
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return NotFound(new { message = "Client not found" });
            }

            return Ok(new
            {
                id = client.Id,
                companyName = client.CompanyName,
                documentNumber = client.DocumentNumber,
                phone = client.Phone,
                address = client.Address
            });
        }
    }
}
