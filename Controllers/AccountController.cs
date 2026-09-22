using InventoryOrderSystem.ViewModels;
using InventoryOrderSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InventoryOrderSystem.Controllers 
{
    public class AccountController(SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> userManager) : Controller
    {
        // Login Function
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await signIn.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsInRoleAsync(user, "Admin"))
                        {
                            return RedirectToAction("Index", "Products");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Shop");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid email or password");
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password");
                }
            }
            return View(model);
        }

        // Logout Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await signIn.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Access Denied 
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}