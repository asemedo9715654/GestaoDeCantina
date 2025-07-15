using GestaoDeCantina.Data;
using GestaoDeCantina.Models;
using GestaoDeCantina.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestaoDeCantina.Controllers
{
    public class ContaController : Controller
    {
        private readonly AuthService _authService;

       

        private readonly ApplicationDbContext _context;

        public ContaController(AuthService authService, ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = _authService.ValidarUsuario(email, senha);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View();
            }

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(string nome, string email, string senha, string confirmarSenha)
        {
            if (senha != confirmarSenha)
            {
                ModelState.AddModelError("", "As senhas não coincidem.");
                return View();
            }

            if (_context.Usuarios.Any(u => u.Email == email))
            {
                ModelState.AddModelError("", "Este email já está em uso.");
                return View();
            }

            var usuario = new Usuario
            {
                Nome = nome,
                Email = email,
                SenhaHash = AuthService.HashSenha(senha)
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Opcional: já logar o usuário após registro
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)

        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Usuario model, string senha)
        {
            if (_context.Usuarios.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Email já cadastrado.");
                return View(model);
            }

            model.SenhaHash = AuthService.HashSenha(senha);
            //model.CriadoPeloAdmin = true;

            _context.Usuarios.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario model)
        {
            var usuario = _context.Usuarios.Find(model.Id);
            if (usuario == null) return NotFound();

            usuario.Nome = model.Nome;
            usuario.Email = model.Email;
            usuario.Role = model.Role;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
