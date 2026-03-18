using System.Text.RegularExpressions;

namespace planificacion.Domain.ValueObjects;

public record Email
{
    public string Valor { get; init; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío", nameof(email));

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 254)
            throw new ArgumentException("El email no puede exceder 254 caracteres", nameof(email));

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("El formato del email no es válido", nameof(email));

        return new Email(email);
    }

    public override string ToString() => Valor;
}
