using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Domain.Models;
using Persistence;
using Persistence.Context;

namespace API.Extensions
{
    public static class SeedDataManager
    {
        public static IHost SeedData(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            using var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                Seed.SeedDataAsync(dataContext, userManager, roleManager).Wait();
            }
            catch (Exception e)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "Wystąpił problem podczas wprowadzania danych");
            }

            return host;
        }
    }
}