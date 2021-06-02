using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Persistence.Context;

namespace Persistence
{
    public static class Seed
    {
        public static async Task SeedDataAsync(DataContext dataContext, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = "Maciek",
                        LastName = "Grzela",
                        UserName = "mgrzela",
                        Email = "maciekgrzela45@gmail.com",
                    }
                };

                if (!roleManager.Roles.Any())
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("Customer"));
                    await roleManager.CreateAsync(new IdentityRole("Owner"));
                }

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "zaq1@WSX");
                    await userManager.AddToRolesAsync(user, new[] {"Admin", "Customer", "Owner"});
                }
            }
        }
    }
}