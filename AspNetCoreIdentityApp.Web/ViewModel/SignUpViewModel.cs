using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModel
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {

        }
        public SignUpViewModel(string userName, string email, string phone, string password)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
            Password = password;
        }

        [Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email Formatı Yanlış")]
        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [Display(Name = "Email :")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Telefon Boş Bırakılamaz")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre Boş Bırakılamaz")]
        [Display(Name = "Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karekter olabilir")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifre Aynı Değildir")]
        [Required(ErrorMessage = "Şifre Tekrar Boş Bırakılamaz")]
        [Display(Name = "Şifre Tekrar:")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karekter olabilir")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
