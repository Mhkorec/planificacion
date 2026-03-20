using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planificacion.Application.Abstractions.Services;
using planificacion.Infraestructure.Identity;
using planificacion.Web.ViewModels.Auth;

namespace planificacion.Web.Controllers;

[AllowAnonymous]
[Route("auth")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;

    public AuthController(
        ILogger<AuthController> logger,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpGet("login")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");
        return View(new LoginVm { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        _logger.LogInformation("Login: intento de inicio de sesión para email {Email}", vm.Email ?? "(null)");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login: validación fallida para {Email}. Errores: {Errors}",
                vm.Email,
                string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(vm);
        }

        var user = await _userManager.FindByEmailAsync(vm.Email!);
        if (user == null)
        {
            _logger.LogWarning("Login: no existe usuario con email {Email}", vm.Email);
            ModelState.AddModelError("", "Credenciales inválidas.");
            return View(vm);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, vm.Password!, isPersistent: vm.RememberMe, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Login: falló para {Email}. Succeeded={Succeeded}, IsLockedOut={IsLockedOut}, IsNotAllowed={IsNotAllowed} (contraseña incorrecta o cuenta no permitida)",
                vm.Email, result.Succeeded, result.IsLockedOut, result.IsNotAllowed);
            if (result.IsNotAllowed)
                ModelState.AddModelError("", "Debes confirmar tu correo electrónico antes de iniciar sesión. Revisa tu bandeja de entrada.");
            else
                ModelState.AddModelError("", "Credenciales inválidas.");
            return View(vm);
        }

        _logger.LogInformation("Login: inicio de sesión exitoso para {Email}. Redirect ReturnUrl={ReturnUrl}",
            vm.Email, vm.ReturnUrl ?? "(ninguna)");

        if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
        {
            _logger.LogInformation("Login: redirigiendo a ReturnUrl {ReturnUrl}", vm.ReturnUrl);
            return Redirect(vm.ReturnUrl);
        }

        _logger.LogInformation("Login: redirigiendo a Home/Index");
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View(new RegisterVm());
    }

    [HttpPost("register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var user = new ApplicationUser
        {
            UserName = vm.Email,
            Email = vm.Email,
            NombreCompleto = $"{vm.Nombre.Trim()} {vm.Apellido.Trim()}".Trim()
        };

        var create = await _userManager.CreateAsync(user, vm.Password);
        if (!create.Succeeded)
        {
            foreach (var err in create.Errors)
                ModelState.AddModelError("", err.Description);
            return View(vm);
        }

        await _userManager.AddToRoleAsync(user, "Usuario");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var callbackUrl = Url.Action(
            "ConfirmEmail",
            "Auth",
            new { userId = user.Id, code = token },
            Request.Scheme)!;

        var cuerpoHtml = $@"
<p style='margin:0 0 1rem 0; font-size:1rem; color:#1a1f36;'>
  Hola <strong>{user.NombreCompleto}</strong>,
</p>
<p style='margin:0 0 1rem 0; font-size:1rem; color:#1a1f36;'>
  Gracias por registrarte en Planificacion. Para activar tu cuenta, haz clic en el siguiente enlace:
</p>
<p style='margin:0 0 1rem 0;'>
  <a href='{callbackUrl}' style='display:inline-block; padding:12px 24px; background:linear-gradient(to right, #00aaff, #8000ff); color:#fff; text-decoration:none; border-radius:8px; font-weight:600;'>Confirmar correo electrónico</a>
</p>
<p style='margin:0; font-size:0.875rem; color:#697386;'>
  Si no creaste esta cuenta, puedes ignorar este mensaje. El enlace expira en 24 horas.
</p>";

        await _emailService.EnviarAsync(
            vm.Email,
            "Confirma tu correo - Planificacion",
            cuerpoHtml);

        return RedirectToAction(nameof(RevisarCorreo), new { email = vm.Email });
    }

    [HttpGet("revisar-correo")]
    public IActionResult RevisarCorreo(string? email = null)
    {
        ViewData["Email"] = email ?? "";
        return View();
    }

    [HttpGet("confirmar-email")]
    public async Task<IActionResult> ConfirmEmail(string? userId, string? code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            TempData["Error"] = "Enlace de confirmación inválido.";
            return RedirectToAction(nameof(Login));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "Usuario no encontrado.";
            return RedirectToAction(nameof(Login));
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            TempData["Error"] = "El enlace ha expirado o ya fue utilizado. Solicita uno nuevo desde el inicio de sesión.";
            return RedirectToAction(nameof(Login));
        }

        TempData["Success"] = "Tu correo ha sido confirmado. Ya puedes iniciar sesión.";
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("denegado")]
    public IActionResult Denegado() => View();
}
