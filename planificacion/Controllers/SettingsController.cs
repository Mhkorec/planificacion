using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace planificacion.Web.Controllers;

[Authorize]
public class SettingsController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Configuración";
        return View();
    }
}
