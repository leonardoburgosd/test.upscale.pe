using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestUpscaleApp.Data;
using TestUpscaleApp.Models;
using TestUpscaleApp.Services;
using TestUpscaleApp.Utilities;

namespace TestUpscaleApp.Pages;

public class LoginModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public LoginModel(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public string? ErrorMessage { get; set; }
    public string? DocumentType { get; set; }
    public bool SessionExpired { get; set; }

    public void OnGet()
    {
        // Verificar si la sesión expiró por inactividad
        if (Request.Query.ContainsKey("sessionExpired") && Request.Query["sessionExpired"] == "true")
        {
            SessionExpired = true;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var documentNumber = Request.Form["username"].ToString();
        var password = Request.Form["password"].ToString();
        var documentType = Request.Form["documentType"].ToString();

        // Validar que los campos no estén vacíos
        if (string.IsNullOrEmpty(documentNumber) || string.IsNullOrEmpty(password))
        {
            ErrorMessage = "Por favor ingresa tu número de documento y contraseña.";
            return Page();
        }

        // Validar que el número de documento sea válido (8-9 dígitos)
        if (!int.TryParse(documentNumber, out int docNumber) || 
            (documentNumber.Length < 8 || documentNumber.Length > 9))
        {
            ErrorMessage = "El número de documento debe tener entre 8 y 9 dígitos.";
            return Page();
        }

        // Buscar el usuario por número de documento
        var user = _context.Users.FirstOrDefault(u => u.DocumentNumber == docNumber);

        if (user == null)
        {
            ErrorMessage = "Usuario o contraseña incorrectos.";
            return Page();
        }

        // Verificar si la cuenta está bloqueada (CVF >= 5)
        if (user.CVF >= 5)
        {
            // Enviar correo notificando el bloqueo
            try
            {
                await _emailService.SendAccountBlockedEmailAsync(user.MainEmail, user.Names);
            }
            catch (Exception ex)
            {
                // Log del error pero continuar con el bloqueo
                Console.WriteLine($"Error al enviar correo de bloqueo: {ex.Message}");
            }
            return RedirectToPage("/BlockSession/BlockSession");
        }

        // Verificar la contraseña
        if (!PasswordHasher.VerifyPassword(password, user.UserPasswordEncrypt, user.UserSalt))
        {
            // Incrementar contador de fallos (CVF)
            user.CVF += 1;
            user.UpdatedAt = DateTime.UtcNow;

            // Si llega a 5 fallos, bloquear la cuenta
            if (user.CVF >= 5)
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                // Enviar correo notificando el bloqueo
                try
                {
                    await _emailService.SendAccountBlockedEmailAsync(user.MainEmail, user.Names);
                }
                catch (Exception ex)
                {
                    // Log del error pero continuar con el bloqueo
                    Console.WriteLine($"Error al enviar correo de bloqueo: {ex.Message}");
                }

                return RedirectToPage("/BlockSession/BlockSession");
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ErrorMessage = "Usuario o contraseña incorrectos.";
            return Page();
        }

        // Contraseña correcta: resetear CVF a 0
        user.CVF = 0;
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        // Guardar información del usuario en sesión
        HttpContext.Session.SetString("UserId", user.Id.ToString());
        HttpContext.Session.SetString("UserName", user.Names);
        HttpContext.Session.SetString("UserEmail", user.MainEmail);

        // Redirigir al perfil
        return RedirectToPage("/Profile/Profile");
    }
}
