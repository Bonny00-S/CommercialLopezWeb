using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Data;
using ProyectoWebCommercialLopez.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ProyectoWebCommercialLopez.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PeopleController : Controller
    {
        private readonly appDbContextCommercial _context;

        public PeopleController(appDbContextCommercial context)
        {
            _context = context;
           
        }

        // GET: People
        public async Task<IActionResult> Index()
        {
            var people = await _context.Person
                .Include(p => p.User)     // ← para traer el rol
                .ToListAsync();

            return View(people);
        }


        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Person
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (person == null) return NotFound();

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person, int Role)
        {

            int userLogged = int.Parse(User.FindFirst("UserId").Value);


            if (_context.Person.Any(p => p.CI == person.CI))
            {
                ModelState.AddModelError("CI", "This Ci is already registered.");
                return View(person);
            }
            if (_context.Person.Any(p => p.Phone == person.Phone))
            {
                ModelState.AddModelError("Phone", "This Phone is already registered.");
                return View(person);
            }


            // Verificar email duplicado
            if (await _context.Person.AnyAsync(p => p.Email == person.Email))
            {
                ModelState.AddModelError("Email", "This email is already registered");
                return View(person);
            }
            if (!ModelState.IsValid)
            {
                return View(person);
            }

            // 1️⃣ Guardar persona primero
            person.CreatedAt = DateTime.Now;
            person.State = 1;
            person.CreatedBy =  userLogged;

            _context.Person.Add(person);
            await _context.SaveChangesAsync();  

  
            string username = GenerateSecureUsername();

            
            string rawPassword = GenerateRandomPassword();

            // 4️⃣ Cifrar contraseña
            string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

            // 5️⃣ Crear objeto User relacionado
            var user = new User
            {
                IdPerson = person.Id,
                Username = username,
                Password = encryptedPassword,
                Role = Role,             // o el rol que quieras
                StatePassword = 1,
                CreatedAt = DateTime.Now,
                CreatedBy = userLogged
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // 6️⃣ Enviar EMAIL con username y password
            await SendUserCredentialsEmail(person.Email!, username, rawPassword);

            ViewBag.Success = true;
            ViewBag.Email = person.Email;

            return View(person);

        }



        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var person = await _context.Person
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (person == null) return NotFound();
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Person person, int Role)
        {
            if (id != person.Id)
                return NotFound();

            // 🔹 VALIDACIÓN CI DUPLICADO
            if (await _context.Person.AnyAsync(p => p.CI == person.CI && p.Id != id))
                ModelState.AddModelError("CI", "This CI is already registered.");

            // 🔹 VALIDACIÓN TELÉFONO DUPLICADO (excluir el mismo ID)
            if (await _context.Person.AnyAsync(p => p.Phone == person.Phone && p.Id != id))
                ModelState.AddModelError("Phone", "This Phone is already registered.");

            // 🔹 VALIDACIÓN EMAIL DUPLICADO
            if (await _context.Person.AnyAsync(p => p.Email == person.Email && p.Id != id))
                ModelState.AddModelError("Email", "This email is already registered.");

            if (!ModelState.IsValid)
                return View(person);

            // 🔥 Obtener original desde DB
            var original = await _context.Person
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (original == null)
                return NotFound();

            // ✅ **DETECTAR SI HUBO CAMBIOS**
            bool noChanges =
                original.Name == person.Name &&
                original.LastName == person.LastName &&
                original.CI == person.CI &&
                original.Phone == person.Phone &&
                original.Email == person.Email &&
                original.Address == person.Address;

            // Además, validar si el rol no cambió
            var userOriginal = await _context.User.FirstOrDefaultAsync(u => u.IdPerson == id);
            bool roleUnchanged = (userOriginal?.Role == Role);

            if (noChanges && roleUnchanged)
            {
                // ✔ No hubo cambios → no guardar nada
                ViewBag.NoChanges = true;
                return View(person);
            }

            // 🔥 ID de usuario logueado
            int userLogged = int.Parse(User.FindFirst("UserId").Value);

            // 📌 Mantener datos de creación
            person.CreatedAt = original.CreatedAt;
            person.CreatedBy = original.CreatedBy;

            // 📌 Datos de modificación
            person.ModifiedAt = DateTime.Now;
            person.ModifiedBy = userLogged;

            // ✔ Actualizar persona
            _context.Person.Update(person);
            await _context.SaveChangesAsync();

            // ✔ Actualizar rol si cambió
            if (userOriginal != null && !roleUnchanged)
            {
                userOriginal.Role = Role;
                userOriginal.ModifiedAt = DateTime.Now;
                userOriginal.ModifiedBy = userLogged;

                _context.User.Update(userOriginal);
                await _context.SaveChangesAsync();
            }

            ViewBag.Success = true;
            return View(person);
        }


        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 1️⃣ Buscar persona
            var person = await _context.Person.FindAsync(id);

            if (person != null)
            {
                // 2️⃣ Buscar usuario relacionado
                var user = await _context.User
                    .FirstOrDefaultAsync(u => u.IdPerson == id);

                // 3️⃣ Eliminar usuario si existe
                if (user != null)
                {
                    _context.User.Remove(user);
                }

                // 4️⃣ Eliminar persona
                _context.Person.Remove(person);

                // 5️⃣ Guardar cambios
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.Id == id);
        }

        #region GENERATION USER AND PASSWORD
        //GENERADOR DE PASSWORD
        public static string GenerateSecureUsername()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";

            string partLetters = "";
            string partNumbers = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[4];

                // 4 letras
                for (int i = 0; i < 4; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    partLetters += letters[(int)(num % letters.Length)];
                }

                // 2 números
                for (int i = 0; i < 2; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    partNumbers += numbers[(int)(num % numbers.Length)];
                }
            }

            return $"usr-{partLetters}{partNumbers}".ToLower();
        }

        public static string GenerateRandomPassword()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            string password = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[4];

                // 3 letras
                for (int i = 0; i < 3; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    password += letters[(int)(num % letters.Length)];
                }

                // 4 números
                for (int i = 0; i < 4; i++)
                {
                    rng.GetBytes(buffer);
                    uint num = BitConverter.ToUInt32(buffer, 0);
                    password += numbers[(int)(num % numbers.Length)];
                }
            }

            return password;
        }

        #endregion

        #region SEND EMAIL
        private async Task SendUserCredentialsEmail(string email, string username, string password)
        {
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Sistema Comercial", "bonnycresporomero@gmail.com"));
            message.To.Add(new MimeKit.MailboxAddress("", email));
            message.Subject = "Credenciales de acceso";

            message.Body = new MimeKit.TextPart("html")
            {
                Text = $@"
    <div style='font-family: Arial, sans-serif; background-color: #f5f5f5; padding: 20px;'>
    
        <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>

            <!-- Encabezado -->
            <div style='background-color: #FF7F00; padding: 15px; text-align: center;'>
                <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>
                    Sistema Commercial López
                </h1>
            </div>

            <!-- Contenido -->
            <div style='padding: 20px; color: #333; font-size: 15px; line-height: 1.6;'>
                <h2 style='color: #000; text-align: center;'>Bienvenido al sistema</h2>

                <p>Estas son tus credenciales de acceso:</p>

                <p><b style='color:#FF7F00;'>Usuario:</b> {username}</p>
                <p><b style='color:#FF7F00;'>Contraseña:</b> {password}</p>

                <p style='margin-top: 15px;'>
                    Por favor, cambia tu contraseña después del primer inicio de sesión
                    para mantener la seguridad de tu cuenta.
                </p>
            </div>

            <!-- Footer -->
            <div style='background-color: #000; color: white; text-align: center; padding: 12px; font-size: 13px;'>
                © {DateTime.Now.Year} Commercial López — Todos los derechos reservados.
            </div>

        </div>
    </div>
"
            };


            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("bonnycresporomero@gmail.com", "klwy lpze wqcu hqtz");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
        #endregion

        public IActionResult ChangePasswordUser()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePasswordUser(string currentPassword, string newPassword, string confirmPassword)
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);

            var user = await _context.User.FirstOrDefaultAsync(u => u.IdPerson == userId);
            if (user == null)
                return RedirectToAction("Login");

            // Validar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                ViewBag.Error = "The current password is incorrect.";
                return View();
            }

            // Validar confirmación
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "The passwords do not match.";
                return View();
            }

            // Validaciones fuertes
            bool strongPassword =
                newPassword.Length >= 8 &&
                newPassword.Any(char.IsUpper) &&
                newPassword.Any(char.IsLower) &&
                newPassword.Any(char.IsDigit) &&
                newPassword.Any(ch => "!@#$%^&*(),.?\":{}|<>".Contains(ch));

            if (!strongPassword)
            {
                ViewBag.Error = "The password does not meet the security requirements.";
                return View();
            }

            // Guardar nueva contraseña
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.StatePassword = 0;
            user.ModifiedAt = DateTime.Now;
            user.ModifiedBy = userId;

            _context.User.Update(user);
            await _context.SaveChangesAsync();

            // 🔥 YA NO SE CIERRA SESIÓN — EL USUARIO SIGUE LOGUEADO NORMALMENTE

            ViewBag.Success = "Your password has been updated successfully.";
            return View();
        }



        
        public async Task<IActionResult> Profile()
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);

            var person = await _context.Person
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (person == null)
                return NotFound();

            return View(person);
        }
       

    }
}
