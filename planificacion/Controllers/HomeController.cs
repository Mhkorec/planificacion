using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using planificacion.Infraestructure.Identity;

namespace planificacion.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserDisplayName"] = user?.NombreCompleto ?? user?.UserName ?? User.Identity.Name ?? "Usuario";
            ViewData["Layout"] = "_HomeLayout";
        }
        else
        {
            ViewData["Layout"] = "_Layout";
        }
        return View();
    }

    public IActionResult Contacto()
    {
        return View();
    }
}
