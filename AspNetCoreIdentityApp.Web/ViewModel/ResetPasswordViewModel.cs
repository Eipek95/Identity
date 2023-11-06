using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModel
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Yeni Şifre :")]
        public string Password { get; set; } = null!;


        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifre Aynı Değildir")]
        [Required(ErrorMessage = "Şifre Tekrar Boş Bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar:")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
