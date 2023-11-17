using AspNetCoreIdentityApp.Core.PermissionsRoot;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Repository.Seeds
{
    public class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
            var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
            var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");
            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

                await AddReadRolePermission(basicRole, roleManager);
            }

            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" });

                var advancedRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;

                await AddReadRolePermission(advancedRole, roleManager);
                await AddCreateAndUpdateRolePermission(advancedRole, roleManager);
            }
            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });

                var adminRole = (await roleManager.FindByNameAsync("AdminRole"))!;

                await AddReadRolePermission(adminRole, roleManager);
                await AddCreateAndUpdateRolePermission(adminRole, roleManager);
                await AddDeleteRolePermission(adminRole, roleManager);
            }
        }



        public async static Task AddReadRolePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Read));
        }

        public async static Task AddCreateAndUpdateRolePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Write));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Write));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Write));

            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Update));
        }


        public async static Task AddDeleteRolePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Delete));
        }
    }
}
