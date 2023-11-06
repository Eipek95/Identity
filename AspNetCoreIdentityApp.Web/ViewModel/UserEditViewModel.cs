using AspNetCoreIdentityApp.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModel
{
    public class UserEditViewModel
    {
        [Required(ErrorMessage = "Kullanıcı Adı Boş Bırakılamaz")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email Formatı Yanlış")]
        [Required(ErrorMessage = "Email Boş Bırakılamaz")]
        [Display(Name = "Email :")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz")]
        [Display(Name = "Telefon Numarası :")]
        public string Phone { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi :")]
        public DateTime? BirthDate { get; set; }


        [Display(Name = "Şehir :")]
        public string? City { get; set; }


        [Display(Name = "Profil Resmi :")]
        public IFormFile? Picture { get; set; }

        [Display(Name = "Cinsiyet :")]
        public Gender? Gender { get; set; }
    }
}
