using EGL.Kinexa.Domain.Constants;
using EGL.Kinexa.Domain.Entities;
using EGL.Kinexa.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EGL.Kinexa.Persistence.Seed;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = scope.ServiceProvider.GetRequiredService<KinexaDbContext>();

        await context.Database.MigrateAsync();

        // Roles
        var roles = new[] { UserRoles.Administrador, UserRoles.CreadorContenido, UserRoles.VisorCotizaciones };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Users
        var adminEmail = "comercial@kinexa.com.mx";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Manuel Alejandro",
                LastName = "Reyes Navarrete"
            };

            var result = await userManager.CreateAsync(adminUser, "Kinexa2024!");
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(adminUser, new[] { UserRoles.Administrador, UserRoles.VisorCotizaciones });
            }
        }

        var creatorEmail = "ana.palma.orthomaster@gmail.com";
        if (await userManager.FindByEmailAsync(creatorEmail) == null)
        {
            var creatorUser = new ApplicationUser
            {
                UserName = creatorEmail,
                Email = creatorEmail,
                EmailConfirmed = true,
                FirstName = "Ana Laura",
                LastName = "Palmar Puerto"
            };

            var result = await userManager.CreateAsync(creatorUser, "Kinexa2024!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(creatorUser, UserRoles.CreadorContenido);
            }
        }

        // Categories
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "SC Medica", Slug = "sc-medica", SortOrder = 1 },
                new Category { Name = "MeiMei", Slug = "meimei", SortOrder = 2 },
                new Category { Name = "LigaTech", Slug = "ligatech", SortOrder = 3 },
                new Category { Name = "NovaSpine", Slug = "novaspine", SortOrder = 4 }
            );
            await context.SaveChangesAsync();
        }

        // Medical Branches
        if (!await context.MedicalBranches.AnyAsync())
        {
            context.MedicalBranches.AddRange(
                new MedicalBranch { Name = "TyO Columna", Slug = "tyo-columna", SortOrder = 1 },
                new MedicalBranch { Name = "TyO Sports Medicine", Slug = "tyo-sports-medicine", SortOrder = 2 },
                new MedicalBranch { Name = "Cirugía General", Slug = "cirugia-general", SortOrder = 3 },
                new MedicalBranch { Name = "Columna/Neurocirugía", Slug = "columna-neurocirugia", SortOrder = 4 }
            );
            await context.SaveChangesAsync();
        }
    }
}
