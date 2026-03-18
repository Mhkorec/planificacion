using Microsoft.EntityFrameworkCore;
using planificacion.Infraestructure;
using planificacion.Infraestructure.Persistence;
using planificacion.Web.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddInfraestructure(builder.Configuration);
builder.Services.AddRazorPages();

builder.Services.Configure<planificacion.Application.Abstractions.EmailOptions>(
    builder.Configuration.GetSection("Email"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

await IdentitySeeder.SeedAsync(app);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
