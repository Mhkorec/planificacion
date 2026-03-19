using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace planificacion.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private static readonly List<ProjectItemViewModel> _projects = new();
    private static readonly List<ProjectTaskItemViewModel> _tasks = new();

    static ProjectsController()
    {
        var idWeb = Guid.NewGuid();
        var idApp = Guid.NewGuid();
        var idDash = Guid.NewGuid();
        _projects.AddRange(new[]
        {
            new ProjectItemViewModel { Id = idWeb, Name = "Sitio web corporativo", Description = "Rediseño y migración del sitio principal.", CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new ProjectItemViewModel { Id = idApp, Name = "App móvil", Description = "Desarrollo de la aplicación para iOS y Android.", CreatedAt = DateTime.UtcNow.AddDays(-12) },
            new ProjectItemViewModel { Id = idDash, Name = "Dashboard interno", Description = "Panel de métricas y reportes para el equipo.", CreatedAt = DateTime.UtcNow.AddDays(-2) }
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

    public IActionResult Index()
    {
        ViewData["Title"] = "Proyectos";
        ViewData["Layout"] = "_HomeLayout";
        return View(_projects.OrderByDescending(p => p.CreatedAt).ToList());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProjectItemViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model?.Name))
        {
            TempData["Error"] = "El nombre del proyecto es obligatorio.";
            return RedirectToAction(nameof(Index));
        }
        var project = new ProjectItemViewModel
        {
            Id = Guid.NewGuid(),
            Name = model.Name.Trim(),
            Description = model.Description?.Trim(),
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
}

public class ProjectItemViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
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
