using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Core.Entities;

namespace TaskManagementSystem.API
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "RegularUser" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var config = services.GetRequiredService<IConfiguration>();

                await SeedUserAsync(userManager, config, "Admin");
                await SeedUserAsync(userManager, config, "RegularUser");
            }
        }

        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, IConfiguration config, string userType)
        {
            var section = config.GetSection($"DefaultUsers:{userType}");
            var email = section["Email"];
            var userName = section["UserName"];
            var password = section["Password"];
            var role = section["Role"];

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);

                    if (userType == "Admin" && !await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
        }
    }
}