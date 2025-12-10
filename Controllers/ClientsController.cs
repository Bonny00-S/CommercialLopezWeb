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
                if (docNumber.Any(c => "<>'\"\\/;=()".Contains(c)))
                {
                    ModelState.AddModelError("DocumentNumber", "El CI/NIT contiene caracteres no permitidos");
                }

                // No puede empezar con cero
                if (docNumber.StartsWith("0"))
                {
                    ModelState.AddModelError("DocumentNumber", "El CI/NIT no puede empezar con cero");
                }

                // Verificar duplicados
                var existingClient = await _context.Client
                    .FirstOrDefaultAsync(c => c.DocumentNumber == docNumber);
                
                if (existingClient != null)
                {
                    ModelState.AddModelError("DocumentNumber", "Ya existe un cliente registrado con este CI/NIT");
                }
            }

            // ============================================
            // VALIDACIONES DE SEGURIDAD Y FORMATO RAZÓN SOCIAL
            // ============================================
            if (!string.IsNullOrWhiteSpace(client.CompanyName))
            {
                var companyName = client.CompanyName.Trim();

                // Validar secuencias peligrosas
                var dangerousPatterns = new[] { "<script", "<iframe", "javascript:", "onerror=", "onclick=", 
                                                "DROP TABLE", "INSERT INTO", "DELETE FROM", "SELECT" };
                if (dangerousPatterns.Any(pattern => companyName.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("CompanyName", "La Razón Social contiene contenido no permitido");
                }

                // Validar caracteres peligrosos
                if (companyName.Any(c => "<>'\"\\/;=".Contains(c)))
                {
                    ModelState.AddModelError("CompanyName", "La Razón Social contiene caracteres no permitidos");
                }

                // No aceptar solo números
                if (System.Text.RegularExpressions.Regex.IsMatch(companyName, @"^[0-9\s.,&-]+$"))
                {
                    ModelState.AddModelError("CompanyName", "La Razón Social debe contener al menos letras");
                }

                // No aceptar solo símbolos
                if (System.Text.RegularExpressions.Regex.IsMatch(companyName, @"^[.,&\s-]+$"))
                {
                    ModelState.AddModelError("CompanyName", "La Razón Social no puede contener solo símbolos");
                }

                // Verificar duplicados por Razón Social
                var existingByName = await _context.Client
                    .FirstOrDefaultAsync(c => c.CompanyName.ToLower() == companyName.ToLower());
                
                if (existingByName != null)
                {
                    ModelState.AddModelError("CompanyName", "Ya existe un cliente con esta Razón Social");
                }
            }

            // Validar formato de teléfono si se proporciona
            if (!string.IsNullOrWhiteSpace(client.Phone))
            {
                var phoneDigits = new string(client.Phone.Where(char.IsDigit).ToArray());
                if (phoneDigits.Length < 7 || phoneDigits.Length > 15)
                {
                    ModelState.AddModelError("Phone", "El teléfono debe tener entre 7 y 15 dígitos");
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
                
                TempData["SuccessMessage"] = "Cliente registrado exitosamente";
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
                return BadRequest(new { message = "Debe proporcionar un número de documento" });
            }

            var client = await _context.Client
                .Where(c => c.DocumentNumber == document.ToUpper() && c.State == 1)
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
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
