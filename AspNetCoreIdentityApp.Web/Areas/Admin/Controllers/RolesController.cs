using AspNetCoreIdentityApp.Repository.Models;
using AspNetCoreIdentityApp.Web.Areas.Admin.Models;
using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                RoleName = x.Name!
            }).ToListAsync();
            return View(roles);
        }

        [HttpGet, Authorize(Roles = "role-action")]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost, Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole()
            {
                Name = request.RoleName
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol Oluşturulmuşur.";
            return RedirectToAction(nameof(RolesController.Index));
        }

        [HttpGet, Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);


            if (roleToUpdate == null) throw new Exception("Güncellenecek bir rol bulunanmamıştır");


            return View(new RoleUpdateViewModel()
            {
                Id = roleToUpdate.Id,
                RoleName = roleToUpdate!.Name!
            });
        }
        [HttpPost, Authorize(Roles = "role-action")]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null) throw new Exception("Güncellenecek bir rol bulunanmamıştır");

            roleToUpdate.Name = request.RoleName;
            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Rol Bilgisi Güncellendi";
            return View();
        }
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roletoDelete = await _roleManager.FindByIdAsync(id);
            if (roletoDelete == null) throw new Exception("Silinecek bir rol bulunanmamıştır");
            var result = await _roleManager.DeleteAsync(roletoDelete);

            if (!result.Succeeded) throw new Exception(result.Errors.Select(x => x.Description).First());

            TempData["SuccessMessage"] = "Rol Silinmiştir";
            return RedirectToAction(nameof(RolesController.Index));
        }
        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            ViewBag.userId = id;
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(currentUser!);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();
            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name!
                };

                if (userRoles.Contains(role.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }
            return View(roleViewModelList);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = (await _userManager.FindByIdAsync(userId))!;

            foreach (var role in requestList)
            {
                if (role.Exist)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}
