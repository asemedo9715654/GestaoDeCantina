using GestaoDeCantina.Models;
using GestaoDeCantina.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeCantina.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(modelo);

            var resultado = await _signInManager.PasswordSignInAsync(modelo.Email, modelo.Senha, modelo.LembrarMe, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Falha ao fazer login. Verifique as credenciais.");
            return View(modelo);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = new ApplicationUser { UserName = modelo.Email, Email = modelo.Email };

            var resultado = await _userManager.CreateAsync(usuario, modelo.Senha);
            if (resultado.Succeeded)
            {
                // Adiciona o usuário padrão à role "Cliente"
                if (!await _roleManager.RoleExistsAsync("Cliente"))
                    await _roleManager.CreateAsync(new IdentityRole("Cliente"));

                await _userManager.AddToRoleAsync(usuario, "Cliente");

                await _signInManager.SignInAsync(usuario, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return View(modelo);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
