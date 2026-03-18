using System.ComponentModel.DataAnnotations;

namespace planificacion.Web.ViewModels.Auth;

public class LoginVm
{
    [Required]
    public string Username { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
