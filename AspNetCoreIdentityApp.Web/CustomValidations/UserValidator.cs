using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var isDigit = int.TryParse(user.UserName![0].ToString(), out _);//biz burada herhangi birşeye atamasını istemediğimiz için out boş bıraktık.memory de herhangi bir alocate edilmeyecek
            //var isNumeric = int.TryParse(user.UserName![0].ToString(), out a);=> burada username in ilk karekterini kontrol eder ilk karekter int ise onu a ya

            if (isDigit)
            {
                errors.Add(new()
                {
                    Code = "UserNameContainFirstLetterDigit",
                    Description = "Kullanıcı Adının ilk karekteri sayısal bir karekter içeremez"
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
