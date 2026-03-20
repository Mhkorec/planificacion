using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace planificacion.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly IWebHostEnvironment _environment;
    private static readonly List<ProjectItemViewModel> _projects = new();
    private static readonly List<ProjectTaskItemViewModel> _tasks = new();
    private const string DefaultProjectIcon = "bi bi-folder-fill";
    private static readonly string[] AllowedProjectIcons =
    {
        "bi bi-folder-fill",
        "bi bi-briefcase-fill",
        "bi bi-bar-chart-fill",
        "bi bi-code-slash",
        "bi bi-megaphone-fill",
        "bi bi-phone-fill",
        "bi bi-palette-fill",
        "bi bi-gear-fill",
        "bi bi-database-fill",
        "bi bi-lightbulb-fill",
        "bi bi-house-fill",
        "bi bi-building-fill",
        "bi bi-shop",
        "bi bi-cart-fill",
        "bi bi-truck",
        "bi bi-airplane-fill",
        "bi bi-car-front-fill",
        "bi bi-bicycle",
        "bi bi-globe-americas",
        "bi bi-geo-alt-fill",
        "bi bi-link-45deg",
        "bi bi-send-fill",
        "bi bi-envelope-fill",
        "bi bi-telephone-fill",
        "bi bi-pie-chart-fill",
        "bi bi-graph-up-arrow",
        "bi bi-cash-stack",
        "bi bi-wallet2",
        "bi bi-list-check",
        "bi bi-check2-square",
        "bi bi-calendar-event-fill",
        "bi bi-clock-fill",
        "bi bi-bell-fill",
        "bi bi-people-fill",
        "bi bi-person-fill",
        "bi bi-hand-thumbs-up-fill",
        "bi bi-terminal-fill",
        "bi bi-server",
        "bi bi-cloud-fill",
        "bi bi-shield-lock-fill",
        "bi bi-lock-fill",
        "bi bi-key-fill",
        "bi bi-tools",
        "bi bi-bug-fill",
        "bi bi-camera-fill",
        "bi bi-camera-video-fill",
        "bi bi-image-fill",
        "bi bi-music-note-beamed",
        "bi bi-headphones",
        "bi bi-controller",
        "bi bi-book-fill",
        "bi bi-mortarboard-fill",
        "bi bi-heart-fill",
        "bi bi-activity",
        "bi bi-tree-fill",
        "bi bi-brush-fill",
        "bi bi-pencil-fill",
        "bi bi-printer-fill",
        "bi bi-file-earmark-text-fill",
        "bi bi-box-seam-fill",
        "bi bi-gift-fill",
        "bi bi-trophy-fill",
        "bi bi-flag-fill",
        "bi bi-star-fill",
        "bi bi-rocket-takeoff-fill",

        // Reacciones (Font Awesome) para variedad tipo "stickers/apps"
        "fa-solid fa-face-smile",
        "fa-solid fa-face-laugh",
        "fa-solid fa-face-laugh-squint",
        "fa-solid fa-face-grin-stars",
        "fa-solid fa-face-grin-hearts",
        "fa-solid fa-face-kiss-wink-heart",
        "fa-solid fa-face-surprise",
        "fa-solid fa-face-meh",
        "fa-solid fa-face-rolling-eyes",
        "fa-solid fa-face-frown",
        "fa-solid fa-face-sad-tear",
        "fa-solid fa-face-angry",
        "fa-solid fa-face-tired",
        "fa-solid fa-face-dizzy",
        "fa-solid fa-face-flushed",
        "fa-solid fa-face-grimace"
    };

    static ProjectsController()
    {
        var idWeb = Guid.NewGuid();
        var idApp = Guid.NewGuid();
        var idDash = Guid.NewGuid();
        _projects.AddRange(new[]
        {
            new ProjectItemViewModel { Id = idWeb, Name = "Sitio web corporativo", Description = "Rediseño y migración del sitio principal.", IconClass = "bi bi-megaphone-fill", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new ProjectItemViewModel { Id = idApp, Name = "App móvil", Description = "Desarrollo de la aplicación para iOS y Android.", IconClass = "bi bi-phone-fill", CreatedAt = DateTime.UtcNow.AddDays(-12) },
            new ProjectItemViewModel { Id = idDash, Name = "Dashboard interno", Description = "Panel de métricas y reportes para el equipo.", IconClass = "bi bi-bar-chart-fill", CreatedAt = DateTime.UtcNow.AddDays(-2) }
        });
        var today = DateTime.UtcNow.Date;
        _tasks.AddRange(new[]
        {
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idWeb, Title = "Diseño de wireframes", Status = "in-progress", DueDate = today.AddDays(7), Priority = "high", Owner = "Alex" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idWeb, Title = "Maquetado HTML/CSS", Status = "not-started", DueDate = today.AddDays(14), Priority = "medium", Owner = "Sarah" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idWeb, Title = "Integración con CMS", Status = "not-started", DueDate = today.AddDays(21), Priority = "high", Owner = "David" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idApp, Title = "Diseño de pantallas", Status = "completed", DueDate = today.AddDays(-2), Priority = "high", Owner = "Kate" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idApp, Title = "API de autenticación", Status = "in-review", DueDate = today.AddDays(5), Priority = "high", Owner = "Alex" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idApp, Title = "Tests E2E", Status = "not-started", DueDate = today.AddDays(12), Priority = "medium", Owner = "Sarah" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idDash, Title = "Definir métricas", Status = "in-progress", DueDate = today.AddDays(3), Priority = "medium", Owner = "David" },
            new ProjectTaskItemViewModel { Id = Guid.NewGuid(), ProjectId = idDash, Title = "Gráficos y reportes", Status = "not-started", DueDate = today.AddDays(10), Priority = "high", Owner = "Alex" }
        });
    }

    public ProjectsController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Proyectos";
        ViewData["Layout"] = "_HomeLayout";
        ViewData["ProjectIcons"] = AllowedProjectIcons;
        return View(_projects.OrderByDescending(p => p.CreatedAt).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectItemViewModel model, IFormFile? IconFile)
    {
        if (string.IsNullOrWhiteSpace(model?.Name))
        {
            TempData["Error"] = "El nombre del proyecto es obligatorio.";
            return RedirectToAction(nameof(Index));
        }

        var uploadedIconPath = await SaveProjectIconAsync(IconFile);
        if (IconFile != null && uploadedIconPath == null)
        {
            TempData["Error"] = "El icono debe ser PNG, JPG, JPEG, WEBP o SVG y pesar menos de 2 MB.";
            return RedirectToAction(nameof(Index));
        }

        var project = new ProjectItemViewModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
            IconClass = NormalizeProjectIcon(model.IconClass),
            IconImagePath = uploadedIconPath,
            CreatedAt = DateTime.UtcNow
        };
        _projects.Add(project);
        TempData["Success"] = "Proyecto creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        if (project != null)
        {
            _projects.Remove(project);
            _tasks.RemoveAll(t => t.ProjectId == id);
            TempData["Success"] = "Proyecto eliminado correctamente.";
        }
        else
        {
            TempData["Error"] = "No se encontró el proyecto.";
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Detail(Guid id)
    {
        var project = _projects.FirstOrDefault(p => p.Id == id);
        if (project == null)
        {
            TempData["Error"] = "Proyecto no encontrado.";
            return RedirectToAction(nameof(Index));
        }
        var projectTasks = _tasks.Where(t => t.ProjectId == id).OrderBy(t => t.DueDate).ToList();
        ViewData["Title"] = project.Name;
        ViewData["Layout"] = "_HomeLayout";
        var model = new ProjectDetailViewModel { Project = project, Tasks = projectTasks };
        return View(model);
    }

    private static string NormalizeProjectIcon(string? iconClass)
    {
        if (string.IsNullOrWhiteSpace(iconClass))
        {
            return DefaultProjectIcon;
        }

        var normalized = iconClass.Trim();
        return AllowedProjectIcons.Contains(normalized, StringComparer.Ordinal)
            ? normalized
            : DefaultProjectIcon;
    }

    private async Task<string?> SaveProjectIconAsync(IFormFile? iconFile)
    {
        if (iconFile == null || iconFile.Length == 0)
        {
            return null;
        }

        const long maxBytes = 2 * 1024 * 1024;
        if (iconFile.Length > maxBytes)
        {
            return null;
        }

        var extension = Path.GetExtension(iconFile.FileName).ToLowerInvariant();
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".webp", ".svg"
        };
        if (!allowedExtensions.Contains(extension))
        {
            return null;
        }

        var folderRelative = Path.Combine("uploads", "project-icons");
        var folderAbsolute = Path.Combine(_environment.WebRootPath, folderRelative);
        Directory.CreateDirectory(folderAbsolute);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fileAbsolutePath = Path.Combine(folderAbsolute, fileName);
        await using var stream = new FileStream(fileAbsolutePath, FileMode.Create);
        await iconFile.CopyToAsync(stream);

        return "/" + folderRelative.Replace("\\", "/") + "/" + fileName;
    }
}

public class ProjectItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string IconClass { get; set; } = "bi bi-folder-fill";
    public string? IconImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProjectTaskItemViewModel
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "not-started"; // not-started, in-progress, in-review, completed
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "medium"; // high, medium, low
    public string Owner { get; set; } = "";
}

public class ProjectDetailViewModel
{
    public ProjectItemViewModel Project { get; set; } = null!;
    public List<ProjectTaskItemViewModel> Tasks { get; set; } = new();
}
