namespace planificacion.Application.Abstractions;

/// <summary>
/// Opciones para el envío de correos (logo y escudos incrustados en el cuerpo).
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// Ruta absoluta al archivo de imagen del logo a incrustar en el cuerpo del email.
    /// Si está vacío o el archivo no existe, no se agrega logo.
    /// </summary>
    public string? LogoPath { get; set; }

    /// <summary>
    /// Ruta absoluta a la imagen del escudo (lado izquierdo del logo).
    /// </summary>
    public string? EscudoDvaPath { get; set; }

    /// <summary>
    /// Ruta absoluta a la imagen del escudo secundario (lado derecho del logo).
    /// </summary>
    public string? EscudoPoliciaPath { get; set; }
}
