namespace TestUpscaleApp.Services;

public class SessionInactivityService
{
    private const int InactivityTimeoutMinutes = 1;
    private const int WarningDurationSeconds = 50;
    private const string LastActivityKey = "LastActivity";
    private const string InactivityWarningShownKey = "InactivityWarningShown";

    /// <summary>
    /// Verifica si la sesión ha estado inactiva por más de 1 minuto
    /// </summary>
    public bool IsSessionInactive(ISession session)
    {
        var lastActivityString = session.GetString(LastActivityKey);
        
        if (string.IsNullOrEmpty(lastActivityString))
        {
            return false;
        }

        if (!DateTime.TryParse(lastActivityString, out DateTime lastActivity))
        {
            return false;
        }

        var inactivityDuration = DateTime.UtcNow - lastActivity;
        return inactivityDuration.TotalMinutes >= InactivityTimeoutMinutes;
    }

    /// <summary>
    /// Determina si debe mostrarse la advertencia de inactividad (entre 1 y 1.83 minutos)
    /// </summary>
    public bool ShouldShowInactivityWarning(ISession session)
    {
        var lastActivityString = session.GetString(LastActivityKey);
        
        if (string.IsNullOrEmpty(lastActivityString))
        {
            return false;
        }

        if (!DateTime.TryParse(lastActivityString, out DateTime lastActivity))
        {
            return false;
        }

        var inactivityDuration = DateTime.UtcNow - lastActivity;
        var minutesInactive = inactivityDuration.TotalMinutes;

        // Mostrar advertencia entre 1 y 1.83 minutos (1 minuto + 50 segundos)
        return minutesInactive >= InactivityTimeoutMinutes && 
               minutesInactive < (InactivityTimeoutMinutes + (WarningDurationSeconds / 60.0));
    }

    /// <summary>
    /// Actualiza la última actividad del usuario y reinicia el contador de inactividad
    /// </summary>
    public void UpdateLastActivity(ISession session)
    {
        session.SetString(LastActivityKey, DateTime.UtcNow.ToString("O"));
        session.Remove(InactivityWarningShownKey);
    }

    /// <summary>
    /// Marca que la advertencia de inactividad ya fue mostrada al usuario
    /// </summary>
    public void MarkWarningAsShown(ISession session)
    {
        session.SetString(InactivityWarningShownKey, "true");
    }

    /// <summary>
    /// Verifica si la advertencia de inactividad ya fue mostrada en esta sesión
    /// </summary>
    public bool HasWarningBeenShown(ISession session)
    {
        var shown = session.GetString(InactivityWarningShownKey);
        return !string.IsNullOrEmpty(shown);
    }

    /// <summary>
    /// Calcula los segundos restantes antes de que expire la sesión (máximo 50 segundos)
    /// </summary>
    public int GetRemainingWarningSeconds(ISession session)
    {
        var lastActivityString = session.GetString(LastActivityKey);
        
        if (string.IsNullOrEmpty(lastActivityString) || 
            !DateTime.TryParse(lastActivityString, out DateTime lastActivity))
        {
            return 0;
        }

        var inactivityDuration = DateTime.UtcNow - lastActivity;
        var secondsInactive = (int)inactivityDuration.TotalSeconds;
        var secondsAtWarning = InactivityTimeoutMinutes * 60;
        var remainingSeconds = WarningDurationSeconds - (secondsInactive - secondsAtWarning);

        return Math.Max(0, remainingSeconds);
    }
}
