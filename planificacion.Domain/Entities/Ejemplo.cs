using planificacion.Domain.Enums;

namespace planificacion.Domain.Entities;

/// <summary>
/// Entidad de ejemplo para planificacion. Reemplazar con entidades del dominio real.
/// </summary>
public class Ejemplo
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public EstadoEjemplo Estado { get; set; } = EstadoEjemplo.Borrador;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
