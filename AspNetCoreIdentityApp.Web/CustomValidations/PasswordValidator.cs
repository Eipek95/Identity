using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();
            //password! => password boş olmayacak
            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new()
                {
                    Code = "PasswordContainUserName",
                    Description = "Şifre Alanı Kullanıcı Adı İçeremez"
                });
            }

            if (password!.ToLower().StartsWith("1234"))
            {
                errors.Add(new()
                {
                    Code = "PasswordContain1234",
                    Description = "Şifre Alanı Ardışık Sayı İçeremez"
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
