using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestUpscaleApp.Data;

namespace TestUpscaleApp.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public string? UserName { get; set; }
    public string? UserSurname { get; set; }
    public string? ErrorMessage { get; set; }
    public bool UserFound { get; set; }

    public void OnGet()
    {
        var code = Request.Query["code"].ToString();

        if (!string.IsNullOrEmpty(code))
        {
            // Buscar el usuario por código de validación
            var user = _context.Users.FirstOrDefault(u => u.ValidationCode == code);

            if (user != null)
            {
                UserName = user.Names;
                UserSurname = user.FathersSurname;
                UserFound = true;
            }
            else
            {
                ErrorMessage = "El código de validación no es válido o ha expirado.";
                UserFound = false;
            }
        }
        else
        {
            UserFound = false;
        }
    }
}
