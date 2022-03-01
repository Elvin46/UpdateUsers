using Fiorella.DataAccessLayer;
using Fiorella.Models;
using Fiorella.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorella.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(AppDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _dbContext.Users.ToListAsync();
            List<UserViewModel> userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                UserViewModel userVM = new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()??"doesn't have role"
                };
                userViewModels.Add(userVM);
            }
            return View(userViewModels);
        }

        public async Task<IActionResult> Activate(string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.IsActive == true)
            {
                user.IsActive = false;
            }
            else
            {
                user.IsActive = true;
            }
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ChangeRole(string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (id == null)
            {
                return NotFound();
            }
            string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (role == null)
            {
                return NotFound();
            }
            ViewBag.CurrentRole = role;

            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string? id,string changedRole)
        {
            if (id == null && changedRole == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            string latestRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (latestRole == null)
            {
                return BadRequest();
            }
            if (latestRole != changedRole)
            {
                var result = await _userManager.AddToRoleAsync(user, changedRole);
                if (!result.Succeeded)
                {

                    ModelState.AddModelError("", "Problems Found");

                }
                var deleteRole = await _userManager.RemoveFromRoleAsync(user, latestRole);
                if (deleteRole.Succeeded)
                {
                    ModelState.AddModelError("", "Problems Found");
                }
                await _userManager.UpdateAsync(user);

            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult ChangePassword(string? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);
            }
            var user = await _userManager.FindByIdAsync(changePasswordVM.Id);
            if (user == null)
            {
                return BadRequest();
            }
            var checkPassword = await _userManager.CheckPasswordAsync(user, changePasswordVM.OldPassword);
            if (checkPassword == false)
            {
                ModelState.AddModelError("OldPassword", "Invalid Password");

            }
            var result = await _userManager.ChangePasswordAsync(user, changePasswordVM.OldPassword, changePasswordVM.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
