using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModel
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [EmailAddress(ErrorMessage = "Email Formatı Yanlış")]
        [Display(Name = "Email:")]
        public string Email { get; set; } = null!;
    }
}
