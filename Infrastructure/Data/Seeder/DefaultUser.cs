using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Seeder;

public static class DefaultUser
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager)
    {
        var user = new IdentityUser
        {
            UserName = "Bezhan",
            Email = "mirzoevbezh@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "777034012",
        };
        
        var existingUser = await userManager.FindByNameAsync(user.UserName);
        if (existingUser != null)
        {
            return;
        }
       var existingUser2 = await userManager.FindByEmailAsync(user.Email);
        if (existingUser2 != null)
        {
            return;
        }

        var result = await userManager.CreateAsync(user, "12345");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
        }
    }
}