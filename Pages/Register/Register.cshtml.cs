using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using TestUpscaleApp.Data;
using TestUpscaleApp.Models;
using TestUpscaleApp.Services;
using TestUpscaleApp.Utilities;

namespace TestUpscaleApp.Pages;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public RegisterModel(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var names = Request.Form["names"].ToString();
        var fathersSurname = Request.Form["fathersSurname"].ToString();
        var mothersSurname = Request.Form["mothersSurname"].ToString();
        var documentType = Request.Form["documentType"].ToString();
        var documentNumber = Request.Form["documentNumber"].ToString();
        var dateOfBirth = Request.Form["dateOfBirth"].ToString();
        var nationality = Request.Form["nationality"].ToString();
        var gender = Request.Form["gender"].ToString();
        var mainEmail = Request.Form["mainEmail"].ToString();
        var secondaryEmail = Request.Form["secondaryEmail"].ToString();
        var mainPhone = Request.Form["mainPhone"].ToString();
        var secondaryPhone = Request.Form["secondaryPhone"].ToString();
        var contractType = Request.Form["contractType"].ToString();
        var hiringDate = Request.Form["hiringDate"].ToString();
        var password = Request.Form["password"].ToString();
        var confirmPassword = Request.Form["confirmPassword"].ToString();

        if (password != confirmPassword)
        {
            ErrorMessage = "Las contraseñas no coinciden.";
            return Page();
        }

        if (!int.TryParse(documentNumber, out int docNumber))
        {
            ErrorMessage = "El número de documento debe ser un número válido.";
            return Page();
        }

        if (!DateTime.TryParse(dateOfBirth, out DateTime dob))
        {
            ErrorMessage = "La fecha de nacimiento no es válida.";
            return Page();
        }

        if (!DateTime.TryParse(hiringDate, out DateTime hDate))
        {
            ErrorMessage = "La fecha de contratación no es válida.";
            return Page();
        }

        var existingUser = _context.Users.FirstOrDefault(u => u.MainEmail == mainEmail);
        if (existingUser != null)
        {
            ErrorMessage = "El email ya está registrado.";
            return Page();
        }

        var validationCode = GenerateValidationCode();
        var (passwordHash, salt) = PasswordHasher.GeneratePasswordHash(password);

        var newUser = new User
        {
            Names = names,
            FathersSurname = fathersSurname,
            MothersSurname = mothersSurname,
            DocumentType = documentType,
            DocumentNumber = docNumber,
            DateOfBirth = dob,
            Nationality = nationality,
            Gender = gender,
            MainEmail = mainEmail,
            SecondaryEmail = string.IsNullOrEmpty(secondaryEmail) ? null : secondaryEmail,
            MainPhone = mainPhone,
            SecondaryPhone = string.IsNullOrEmpty(secondaryPhone) ? null : secondaryPhone,
            ContractType = contractType,
            HiringDate = hDate,
            UserPasswordEncrypt = passwordHash,
            UserSalt = salt,
            CVF = 0,
            ValidationCode = validationCode
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        // Enviar correo de validación
        try
        {
            var mainUrl = $"{Request.Scheme}://{Request.Host}";
            await _emailService.SendValidationEmailAsync(mainEmail, validationCode, mainUrl);
            SuccessMessage = $"Registro exitoso. Se ha enviado un código de validación a {mainEmail}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Registro completado, pero hubo un error al enviar el correo: {ex.Message}";
            return Page();
        }

        return Page();
    }

    private string GenerateValidationCode()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] tokenData = new byte[6];
            rng.GetBytes(tokenData);
            return BitConverter.ToString(tokenData).Replace("-", "").Substring(0, 12);
        }
    }
}
