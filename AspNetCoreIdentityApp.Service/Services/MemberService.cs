﻿using AspNetCoreIdentityApp.Core.Models;
using AspNetCoreIdentityApp.Core.ViewModel;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class MemberService : IMemberService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;

        public MemberService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
        }

        public async Task<UserViewModel> GetUserViewModelByUserNameAsync(string userName)
        {
            //var userClaims = User.Claims.ToList();// HttpContext.User.Claims.ToList(); --->controller olmadan bir business veya başka bir class üzerinden erişmek istersek
            //var email = User.Claims.FirstOrDefault(x=>x.Type==ClaimTypes.Email);

            var currentUser = (await _userManager.FindByNameAsync(userName))!;
            return new UserViewModel
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync(); //hangi sayfaya yönlendirileceğini _navbarlogin içinde belirledik
        }


        public async Task<bool> CheckPasswordAsync(string userName, string password)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;
            return await _userManager.CheckPasswordAsync(currentUser, password);
        }

        public async Task<(bool, IEnumerable<IdentityError>?)> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;
            var resultChangePassword = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);

            if (!resultChangePassword.Succeeded)
            {
                return (false, resultChangePassword.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);//kullanıcının bilgileri değişince var olan oturumlardan yarım saat sonra çıkış yapmasını sağlamak ve senk sağ amacıyla yazıldı
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, newPassword, true, false);

            return (true, null);

        }



        public async Task<UserEditViewModel> GetUserEditViewModelAsync(string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName));

            return new UserEditViewModel()
            {
                UserName = currentUser!.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };
        }

        public SelectList GetGenderSelectList()
        {
            return new SelectList(Enum.GetNames(typeof(Gender)));
        }


        public async Task<(bool, IEnumerable<IdentityError>?)> EditUserAsync(UserEditViewModel request, string userName)
        {
            var currentUser = (await _userManager.FindByNameAsync(userName))!;

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;
            if (request.Picture != null && request.Picture.Length > 0)
            {

                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var result = currentUser.Picture;
                if (!result.IsNullOrEmpty())
                {
                    string fullPath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, result);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                }

                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";
                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);
                using var stream = new FileStream(newPicturePath, FileMode.Create);

                await request.Picture.CopyToAsync(stream);

                currentUser.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                return (false, updateToUserResult.Errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] { new Claim("birthdate", currentUser.BirthDate!.Value.ToString()) });//değişiklik yapınca bilgilerde doğum tarihini cookie ye ekle.claims olarak ekle
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);//normal ekle cookieye
            }
            return (true, null);
        }

        public List<ClaimViewModel> GetClaims(ClaimsPrincipal principal)
        {
            return principal.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
        }

    }
}
