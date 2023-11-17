using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModel
{

    public class SignInViewModel
    {
        public SignInViewModel()
        {
        }
        public SignInViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [EmailAddress(ErrorMessage = "Email Formatı Yanlış")]
        [Display(Name = "Email:")]
        public string Email { get; set; } = null!;
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karekter olabilir")]
        public string Password { get; set; } = null!;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
