using System.Net;
using System.Net.Mail;

namespace TestUpscaleApp.Services;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderPassword;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration)
    {
        // Obtener credenciales desde appsettings.json y luego resolver variables de entorno
        var smtpConfig = configuration.GetSection("Notification");
        
        var serverKey = smtpConfig["host"];
        _smtpServer = Environment.GetEnvironmentVariable(serverKey) 
            ?? throw new InvalidOperationException($"La variable de entorno '{serverKey}' no está configurada.");
        
        var emailKey = smtpConfig["email"];
        _senderEmail = Environment.GetEnvironmentVariable(emailKey) 
            ?? throw new InvalidOperationException($"La variable de entorno '{emailKey}' no está configurada.");
        
        var passwordKey = smtpConfig["password"];
        _senderPassword = Environment.GetEnvironmentVariable(passwordKey) 
            ?? throw new InvalidOperationException($"La variable de entorno '{passwordKey}' no está configurada.");
        
        var portKey = smtpConfig["port"];
        var portStr = Environment.GetEnvironmentVariable(portKey);
        _smtpPort = int.Parse(portStr);
        
        _enableSsl = true;
    }

    public async Task SendValidationEmailAsync(string recipientEmail, string validationCode, string mainUrl)
    {
        try
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = _enableSsl;
                client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                var validationUrl = $"{mainUrl}?code={validationCode}";
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Código de Validación - Registro",
                    Body = GenerateEmailBody(validationCode, validationUrl),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al enviar correo: {ex.Message}", ex);
        }
    }

    public async Task SendAccountBlockedEmailAsync(string recipientEmail, string userName)
    {
        try
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = _enableSsl;
                client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail),
                    Subject = "Cuenta Bloqueada - Acceso Restringido",
                    Body = GenerateBlockedAccountEmailBody(userName),
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al enviar correo: {ex.Message}", ex);
        }
    }

    private string GenerateEmailBody(string validationCode, string validationUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #A91D3A; color: white; padding: 20px; text-align: center; border-radius: 4px 4px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .code-box {{ background-color: #fff; border: 2px solid #0066CC; padding: 15px; text-align: center; margin: 20px 0; border-radius: 4px; }}
        .code {{ font-size: 24px; font-weight: bold; color: #0066CC; letter-spacing: 2px; }}
        .footer {{ background-color: #f0f0f0; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 4px 4px; }}
        .button {{ display: inline-block; background-color: #0066CC; color: white; padding: 12px 30px; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Bienvenido</h1>
        </div>
        <div class='content'>
            <p>Gracias por registrarse. Para completar su registro, debe validar su correo electrónico.</p>
            
            <p>Su código de validación es:</p>
            <div class='code-box'>
                <div class='code'>{validationCode}</div>
            </div>
            
            <p>O haga clic en el siguiente enlace para validar su cuenta:</p>
            <p style='text-align: center;'>
                <a href='{validationUrl}' class='button'>Validar Cuenta</a>
            </p>
            
            <p style='color: #666; font-size: 12px;'>
                Si no realizó este registro, ignore este correo.
            </p>
        </div>
        <div class='footer'>
            <p>Presidencia del Consejo de Ministros - Centro Nacional Requerimientos Especiales</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateBlockedAccountEmailBody(string userName)
    {
        return $@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #A91D3A; color: white; padding: 20px; text-align: center; border-radius: 4px 4px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .alert-box {{ background-color: #fff3cd; border: 2px solid #A91D3A; padding: 15px; text-align: center; margin: 20px 0; border-radius: 4px; }}
        .alert-title {{ font-size: 18px; font-weight: bold; color: #A91D3A; margin-bottom: 10px; }}
        .footer {{ background-color: #f0f0f0; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 4px 4px; }}
        .support-link {{ color: #0066CC; text-decoration: none; font-weight: 600; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Notificación de Seguridad</h1>
        </div>
        <div class='content'>
            <p>Estimado/a {userName},</p>
            
            <div class='alert-box'>
                <div class='alert-title'>⚠️ Su cuenta ha sido bloqueada</div>
                <p>Por razones de seguridad, su cuenta ha sido bloqueada después de múltiples intentos fallidos de inicio de sesión.</p>
            </div>
            
            <p>Si usted realizó estos intentos y desea desbloquear su cuenta, por favor contacte con el área de soporte.</p>
            
            <p>Si no realizó estos intentos, le recomendamos cambiar su contraseña inmediatamente.</p>
            
            <p style='text-align: center; margin-top: 20px;'>
                <a href='#' class='support-link'>Contactar con Soporte</a>
            </p>
        </div>
        <div class='footer'>
            <p>Presidencia del Consejo de Ministros - Centro Nacional Requerimientos Especiales</p>
        </div>
    </div>
</body>
</html>";
    }
}
