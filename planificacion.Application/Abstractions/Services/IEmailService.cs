namespace planificacion.Application.Abstractions.Services;

/// <summary>
/// Abstracción del envío de correo. Implementado en Infraestructure.
/// </summary>
public interface IEmailService
{
    Task EnviarAsync(string para, string asunto, string cuerpoHtml, CancellationToken cancellationToken = default);
}
