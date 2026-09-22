using Microsoft.AspNetCore.Identity;
using InventoryOrderSystem.Models;

namespace InventoryOrderSystem.Data
{
    static public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if(await userManager.FindByEmailAsync("admin@test.com") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "admin@test.com";
                user.Email = "admin@test.com";
                user.EmailConfirmed = true;
                var result = await userManager.CreateAsync(user, "Admin1234");
                if (result.Succeeded) 
                { 
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
            if(await userManager.FindByEmailAsync("user@test.com") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "user@test.com";
                user.Email = "user@test.com";
                user.EmailConfirmed = true;
                var result = await userManager.CreateAsync(user, "User123");
                if (result.Succeeded) 
                { 
                    await userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
            if (await userManager.FindByEmailAsync("user2@test.com") == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.UserName = "user2@test.com";
                user.Email = "user2@test.com";
                user.EmailConfirmed = true;

                var result = await userManager.CreateAsync(user, "User123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}