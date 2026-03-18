using System.ComponentModel.DataAnnotations;

namespace planificacion.Web.ViewModels.Auth;

public class RegisterVm
{
    [Required]
    public string NombreCompleto { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(8), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required, Compare(nameof(Password)), DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = "";
}
