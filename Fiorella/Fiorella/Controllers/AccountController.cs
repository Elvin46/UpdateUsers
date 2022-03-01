using Fiorella.Models;
using Fiorella.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Fiorella.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existUser = await _userManager.FindByNameAsync(registerModel.Username);
            if (existUser != null)
            {
                ModelState.AddModelError("Username", "This Username already exist");
                return View();
            }
            var user = new User
            {
                Fullname = registerModel.Fullname,
                UserName = registerModel.Username,
                Email = registerModel.Email
            };
            await _userManager.AddToRoleAsync(user, "User");
            var result = await _userManager.CreateAsync(user, registerModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var existUser = await _userManager.FindByNameAsync(loginModel.Username);
            if (existUser == null)
            {
                ModelState.AddModelError("","Invalid Username or Password");
                return View();
            }
            await _userManager.AddToRoleAsync(existUser, "User");
            var result = await _signInManager.PasswordSignInAsync(existUser, loginModel.Password, loginModel.RememberMe, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("","This user is locked out");
                return View();

            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ResetPasswordViewModel resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
            {
                return NotFound();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string link = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token }, Request.Scheme, Request.Host.ToString());

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("codep320@gmail.com", "Fiorello");
            msg.To.Add(user.Email);


            msg.Body = $"<a href=\"{link}\">Please reset password with this link</a>";
            msg.Subject = "ResetPassword";
            msg.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("codep320@gmail.com", "codeacademyp320");
            smtp.Send(msg);

            return View();
        }
        public async Task<IActionResult> ResetPassword(string email,string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            var resetPassword = new ResetPasswordViewModel()
            {
                Token = token,
                Email = email
            };
            return View(resetPassword);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordViewModel.Email);
            if (user == null)
            {
                return BadRequest();
            }
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var resetVM = new ResetPasswordViewModel()
            {
                Token = resetPasswordViewModel.Token,
                Email = resetPasswordViewModel.Email
            };
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(resetVM);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
