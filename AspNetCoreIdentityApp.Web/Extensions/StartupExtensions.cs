using AspNetCoreIdentityApp.Web.CustomValidations;
using AspNetCoreIdentityApp.Web.Localization;
using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(2);//şifremi unuttum ile gelen token yasam süreci
            });


            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefgijklmnoprstuwxyz1234567890_";//izin verilenler

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;

                //başarısız girişte kitlenme
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;

            }).AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
