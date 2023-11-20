using AspNetCoreIdentityApp.Core.ViewModel;
using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Service.Services;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        private readonly IMemberService _memberService;
        private string userName => User.Identity!.Name!;
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider, IMemberService memberService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _memberService = memberService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _memberService.GetUserViewModelByUserNameAsync(userName));
        }
        public async Task LogoutAsync()
        {
            await _memberService.LogoutAsync();
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
            if (!await _memberService.CheckPasswordAsync(userName, request.PasswordOld))
            {
                ModelState.AddModelError(string.Empty, "Eski Şifreniz Yanlış");
                return View();
            }

            var (isSuccess, errors) = await _memberService.ChangePasswordAsync(userName, request.PasswordOld, request.PasswordNew);
            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }

            TempData["SuccessMessage"] = "Şifreniz Başarıyla Değiştirilmiştir.";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = _memberService.GetGenderSelectList();
            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            ViewBag.genderList = _memberService.GetGenderSelectList();

            var (isSuccess, errors) = await _memberService.EditUserAsync(request, userName);



            if (!isSuccess)
            {
                ModelState.AddModelErrorList(errors!);
                return View();
            }

            TempData["SuccessMessage"] = "Üye Bilgileri Başarıyla Değiştirilmiştir.";

            return View(await _memberService.GetUserEditViewModelAsync(userName));
        }
        [HttpGet]
        public IActionResult Claims()
        {
            return View(_memberService.GetClaims(User));
        }


        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {
            return View();
        }

        [Authorize(Policy = "VioloncePolicy")]
        [HttpGet]
        public IActionResult VioloncePage()
        {
            return View();
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;

            message = "Bu Sayfayı Görmek için yetkiniz yoktur.Yetki Almak için yöneticiniz ile görüşebilirsiniz.";
            ViewBag.message = message;
            return View();
        }
    }
}
