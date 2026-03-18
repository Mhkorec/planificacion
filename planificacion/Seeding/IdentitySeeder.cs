using Microsoft.AspNetCore.Identity;
using planificacion.Infraestructure.Identity;

namespace planificacion.Web.Seeding;

public static class IdentitySeeder
{
    private static readonly string[] Roles = ["Admin", "Usuario"];

    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = app.Configuration["Seed:AdminEmail"] ?? "adminplanificacion@sanluis.gov.ar";
        var adminPassword = app.Configuration["Seed:AdminPassword"] ?? "Admin1234!"; //https://bitwarden.com/password-generator/#password-generator 

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NombreCompleto = "Administrador"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
