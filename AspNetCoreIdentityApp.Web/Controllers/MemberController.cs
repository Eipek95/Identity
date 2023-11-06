using AspNetCoreIdentityApp.Web.Extensions;
using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            var userViewModel = new UserViewModel
            {
                Email = currentUser!.Email,
                UserName = currentUser!.UserName,
                PhoneNumber = currentUser!.PhoneNumber,
            };
            return View(userViewModel);
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync(); //hangi sayfaya yönlendirileceğini _navbarlogin içinde belirledik
        }

        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, request.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski Şifreniz Yanlış");
                return View();
            }
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, request.PasswordOld, request.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x => x.Description).ToList());
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);//kullanıcının bilgileri değişince var olan oturumlardan yarım saat sonra çıkış yapmasını sağlamak ve senk sağ amacıyla yazıldı
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, request.PasswordNew, true, false);
            TempData["SuccessMessage"] = "Şifreniz Başarıyla Değiştirilmiştir.";
            return View();
        }



        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
            return View(userEditViewModel);
        }
    }
}
