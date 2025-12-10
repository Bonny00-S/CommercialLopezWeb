using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Data;
using ProyectoWebCommercialLopez.Models;
using System.Security.Claims;

namespace ProyectoWebCommercialLopez.Controllers
{
  
    public class AuthController : Controller
    {

        private readonly appDbContextCommercial _context;
        public AuthController(appDbContextCommercial context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Username == userName);

            if (user == null)
            {
                ViewBag.Error = "credentials invalid";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.Error = "credentials invalid";
                return View();
            }

            string roleName = user.Role switch
            {
                1 => "Admin",
                2 => "Cashier",
                3 => "Almacen",
                _ => "Unknown"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.IdPerson.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("StatePassword", user.StatePassword.ToString())
            };


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,  
                ExpiresUtc = DateTime.UtcNow.AddHours(5)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
            if (user.StatePassword == 1)
            {
                return RedirectToAction("changepassword", "Auth");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);

            var user = await _context.User.FirstOrDefaultAsync(u => u.IdPerson == userId);
            if (user == null) return RedirectToAction("Login");

            // Validar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                ViewBag.Error = "The current password is incorrect.";
                return View();
            }

            // Validar confirmación
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "The current password is incorrect.";
                return View();
            }

            // Validaciones fuertes
            var strongPassword =
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

            // 🔥 Cerrar sesión actual para eliminar el claim viejo
            await HttpContext.SignOutAsync();

            // 🔥 Crear nuevos claims SIN StatePassword = 1
            await SignInAgain(user);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email)
        {
            var user = await _context.Person.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ViewBag.Error = "There is no user with that email address..";
                return View();
            }

            // Crear token
            string token = Guid.NewGuid().ToString();

            // Guardar token en base de datos
            var resetToken = new PasswocrdResetToken
            {
                Email = email,
                Token = token,
                Expiration = DateTime.Now.AddMinutes(15)
            };

            _context.PasswordResetToken.Add(resetToken);
            await _context.SaveChangesAsync();

            // Crear enlace
            string resetLink = Url.Action("SetNewPassword", "Auth", new { token }, Request.Scheme);

            await SendResetEmail(email, resetLink);

            ViewBag.Success = "A recovery link was sent to your email.";
            return View();
        }


        // ======================== SEND EMAIL ==========================

        private async Task SendResetEmail(string email, string resetLink)
        {
            var message = new MimeKit.MimeMessage();
            message.From.Add(new MimeKit.MailboxAddress("Sistema Comercial", "bonnycresporomero@gmail.com"));
            message.To.Add(new MimeKit.MailboxAddress("", email));
            message.Subject = "Recuperación de Contraseña - Commercial López";

            message.Body = new MimeKit.TextPart("html")
            {
                Text = $@"
            <div style='font-family: Arial; background:#1a1a1a; padding:20px;'>
                <div style='max-width:600px; margin:auto; background:#2a2a2a; 
                            border-radius:10px; overflow:hidden; color:#fff;'>

                    <div style='background:#ff7f00; padding:15px; text-align:center;'>
                        <h1 style='margin:0;'>Recuperación de Contraseña</h1>
                    </div>

                    <div style='padding:25px;'>
                        <p>Haz clic en el botón para restablecer tu contraseña:</p>

                        <div style='text-align:center; margin:25px 0;'>
                            <a href='{resetLink}' style='background:#ff7f00; color:white; 
                               padding:12px 20px; text-decoration:none; border-radius:6px;'>
                                Restablecer Contraseña
                            </a>
                        </div>

                        <p>Si no solicitaste esto, ignora el mensaje.</p>
                    </div>

                    <div style='background:#000; padding:10px; text-align:center; font-size:12px;'>
                        © {DateTime.Now.Year} Commercial López
                    </div>

                </div>
            </div>"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("bonnycresporomero@gmail.com", "klwy lpze wqcu hqtz");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }


        // ======================== SET NEW PASSWORD (GET) ==========================

        public IActionResult SetNewPassword(string token)
        {
            var storedToken = _context.PasswordResetToken
                .FirstOrDefault(t => t.Token == token && t.Expiration > DateTime.Now);

            if (storedToken == null)
            {
                return Content("Invalid or expired token.");
            }

            ViewBag.Email = storedToken.Email;
            return View();
        }


        // ======================== SET NEW PASSWORD (POST) ==========================

        [HttpPost]
        public async Task<IActionResult> SetNewPassword(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "The passwords do not match.";
                ViewBag.Email = email;
                return View();
            }

            var person = await _context.Person.FirstOrDefaultAsync(u => u.Email == email);
            var user = await _context.User.FirstOrDefaultAsync(u => u.IdPerson == person.Id);

            if (user == null)
            {
                ViewBag.Error = "The user does not exist.";
                return View();
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ModifiedAt = DateTime.Now;
            user.ModifiedBy = user.IdPerson;
            user.StatePassword = 0;

            await _context.SaveChangesAsync();

            // Borrar todos los tokens usados
            var tokens = _context.PasswordResetToken.Where(t => t.Email == email);
            _context.PasswordResetToken.RemoveRange(tokens);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");


        }

        private async Task SignInAgain(User user)
        {
            string roleName = user.Role switch
            {
                1 => "Admin",
                2 => "Cashier",
                3 => "Almacen",
                _ => "Unknown"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.IdPerson.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("StatePassword", user.StatePassword.ToString()) // ahora será 0
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(5)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
        }


    }
}
