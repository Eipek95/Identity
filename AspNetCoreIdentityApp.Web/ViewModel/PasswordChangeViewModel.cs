using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModel
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Eski Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karekter olabilir")]
        public string PasswordOld { get; set; } = null!;


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni Şifre Boş Bırakılamaz")]
        [Display(Name = "Yeni Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karekter olabilir")]

        public string PasswordNew { get; set; } = null!;


        [DataType(DataType.Password)]
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifre Aynı Değildir.")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar Boş Bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar:")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karekter olabilir")]
        public string PasswordNewConfirm { get; set; } = null!;
    }
}
