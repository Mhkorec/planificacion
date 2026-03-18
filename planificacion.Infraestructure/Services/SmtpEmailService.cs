using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using planificacion.Application.Abstractions;
using planificacion.Application.Abstractions.Services;

namespace planificacion.Infraestructure.Services;

public class SmtpEmailService : IEmailService
{
    private const string LogoContentId = "planificacion_logo";
    private const string EscudoDvaContentId = "planificacion_escudo_dva";
    private const string EscudoPoliciaContentId = "planificacion_escudo_policia";
    private const int LogoWidth = 420;

    private readonly IConfiguration _configuration;
    private readonly EmailOptions _emailOptions;

    public SmtpEmailService(IConfiguration configuration, IOptions<EmailOptions> emailOptions)
    {
        _configuration = configuration;
        _emailOptions = emailOptions.Value;
    }

    public async Task EnviarAsync(string para, string asunto, string cuerpoHtml, CancellationToken cancellationToken = default)
    {
        var host = _configuration["Smtp:Host"];
        var portValue = _configuration["Smtp:Port"];
        var user = _configuration["Smtp:User"];
        var pass = _configuration["Smtp:Pass"];
        var from = _configuration["Smtp:From"];

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(portValue) ||
            string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass) ||
            string.IsNullOrWhiteSpace(from))
        {
            throw new InvalidOperationException("Configuración SMTP incompleta.");
        }

        if (!int.TryParse(portValue, out var port))
        {
            throw new InvalidOperationException("Puerto SMTP inválido.");
        }

        var enableSsl = _configuration.GetValue<bool>("Smtp:EnableSsl");

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(user, pass)
        };

        var useLogo = !string.IsNullOrWhiteSpace(_emailOptions.LogoPath) && File.Exists(_emailOptions.LogoPath);
        var useEscudoDva = !string.IsNullOrWhiteSpace(_emailOptions.EscudoDvaPath) && File.Exists(_emailOptions.EscudoDvaPath);
        var useEscudoPolicia = !string.IsNullOrWhiteSpace(_emailOptions.EscudoPoliciaPath) && File.Exists(_emailOptions.EscudoPoliciaPath);

        using var mail = new MailMessage(from, para, asunto, useLogo ? string.Empty : cuerpoHtml)
        {
            IsBodyHtml = true
        };

        if (useLogo)
        {
            var logoStyle = $"max-width:{LogoWidth}px; width:{LogoWidth}px; height:auto; -webkit-user-select:none; user-select:none; pointer-events:none; -webkit-user-drag:none;";
            var logoCentral = $@"<img src=""cid:{LogoContentId}"" alt=""Planificacion"" width=""{LogoWidth}"" style=""{logoStyle}"" draggable=""false"" />";

            var fullHtml = $@"
<!DOCTYPE html>
<html>
<head><meta charset=""utf-8"" /></head>
<body style=""margin:0; padding:24px; background-color:#e8f4f8; font-family:Arial,sans-serif;"">
  <div style=""max-width:560px; margin:0 auto; background-color:#ffffff; border-radius:12px; box-shadow:0 4px 20px rgba(0,0,0,0.08); overflow:hidden;"">
    <div style=""text-align:center; padding:24px 24px 16px 24px;"">
      {logoCentral}
    </div>
    <div style=""padding:0 24px 24px 24px;"">{cuerpoHtml}</div>
  </div>
</body>
</html>";
            var alternateView = AlternateView.CreateAlternateViewFromString(fullHtml, null, "text/html");
            var linkedResource = new LinkedResource(_emailOptions.LogoPath!, "image/png")
            {
                ContentId = LogoContentId
            };
            alternateView.LinkedResources.Add(linkedResource);
            if (useEscudoDva)
            {
                var resDva = new LinkedResource(_emailOptions.EscudoDvaPath!, "image/png") { ContentId = EscudoDvaContentId };
                alternateView.LinkedResources.Add(resDva);
            }
            if (useEscudoPolicia)
            {
                var resPolicia = new LinkedResource(_emailOptions.EscudoPoliciaPath!, "image/png") { ContentId = EscudoPoliciaContentId };
                alternateView.LinkedResources.Add(resPolicia);
            }
            mail.AlternateViews.Add(alternateView);
        }

        await client.SendMailAsync(mail, cancellationToken);
    }
}
