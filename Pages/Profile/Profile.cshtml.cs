using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestUpscaleApp.Data;
using TestUpscaleApp.Services;

namespace TestUpscaleApp.Pages;

public class ProfileModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly SessionInactivityService _inactivityService;

    public ProfileModel(ApplicationDbContext context, SessionInactivityService inactivityService)
    {
        _context = context;
        _inactivityService = inactivityService;
    }

    public string? UserName { get; set; }
    public string? UserNames { get; set; }
    public string? UserFathersSurname { get; set; }
    public string? UserMothersSurname { get; set; }
    public string? UserDocumentType { get; set; }
    public int? UserDocumentNumber { get; set; }
    public DateTime? UserDateOfBirth { get; set; }
    public string? UserNationality { get; set; }
    public string? UserGender { get; set; }
    public string? UserEmail { get; set; }
    public string? UserSecondaryEmail { get; set; }
    public string? UserMainPhone { get; set; }
    public string? UserSecondaryPhone { get; set; }
    public string? UserContractType { get; set; }
    public DateTime? UserHiringDate { get; set; }
    public string? UserRole { get; set; }
    public string? UserOrganization { get; set; }
    public bool ShowInactivityWarning { get; set; }
    public int RemainingSeconds { get; set; }
    public string? FullUserName { get; set; }

    public IActionResult OnGet()
    {
        // Obtener el ID del usuario de la sesión
        var userIdString = HttpContext.Session.GetString("UserId");

        if (string.IsNullOrEmpty(userIdString))
        {
            return RedirectToPage("/Login/Login");
        }

        if (!long.TryParse(userIdString, out long userId))
        {
            return RedirectToPage("/Login/Login");
        }

        // Verificar si la sesión está inactiva
        if (_inactivityService.IsSessionInactive(HttpContext.Session))
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login/Login");
        }

        // Verificar si debe mostrar advertencia de inactividad
        ShowInactivityWarning = _inactivityService.ShouldShowInactivityWarning(HttpContext.Session);
        if (ShowInactivityWarning && !_inactivityService.HasWarningBeenShown(HttpContext.Session))
        {
            RemainingSeconds = _inactivityService.GetRemainingWarningSeconds(HttpContext.Session);
            _inactivityService.MarkWarningAsShown(HttpContext.Session);
        }

        // Buscar el usuario en la base de datos
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return RedirectToPage("/Login/Login");
        }

        // Cargar los datos del usuario en las propiedades
        UserName = user.Names;
        UserNames = user.Names;
        UserFathersSurname = user.FathersSurname;
        UserMothersSurname = user.MothersSurname;
        UserDocumentType = user.DocumentType;
        UserDocumentNumber = user.DocumentNumber;
        UserDateOfBirth = user.DateOfBirth;
        UserNationality = user.Nationality;
        UserGender = user.Gender;
        UserEmail = user.MainEmail;
        UserSecondaryEmail = user.SecondaryEmail;
        UserMainPhone = user.MainPhone;
        UserSecondaryPhone = user.SecondaryPhone;
        UserContractType = user.ContractType;
        UserHiringDate = user.HiringDate;
        UserRole = "Operador";
        UserOrganization = "Ministerio de Salud";
        FullUserName = $"{user.Names} {user.FathersSurname}";
        
        // Pasar el nombre completo a ViewData para el Layout
        ViewData["FullUserName"] = FullUserName;

        // Actualizar última actividad
        _inactivityService.UpdateLastActivity(HttpContext.Session);

        return Page();
    }

    public IActionResult OnPostExtendSession()
    {
        // Actualizar última actividad para extender la sesión
        _inactivityService.UpdateLastActivity(HttpContext.Session);
        return new JsonResult(new { success = true });
    }
}
