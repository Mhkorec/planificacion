using Microsoft.AspNetCore.Identity;

namespace planificacion.Infraestructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? NombreCompleto { get; set; }
}
