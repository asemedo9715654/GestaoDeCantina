using GestaoDeCantina.Data;
using GestaoDeCantina.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeCantina.Controllers
{
    // Controllers/SenhaController.cs
    [Authorize]
    public class SenhaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SenhaController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Escolher()
        {
            var hoje = DateTime.Today;
            var pratos = _context.Pratos
                .Where(p => p.Dia == hoje)
                .ToList();

            return View(pratos);
        }

        [HttpPost]
        public async Task<IActionResult> Escolher(int pratoId)
        {
            var user = await _userManager.GetUserAsync(User);
            var senha = new Senha
            {
                UsuarioId = user.Id,
                PratoId = pratoId,
                DataEscolha = DateTime.Now
            };

            _context.Senhas.Add(senha);
            await _context.SaveChangesAsync();

            return RedirectToAction("MinhaSenha");
        }

        public async Task<IActionResult> MinhaSenha()
        {
            var user = await _userManager.GetUserAsync(User);
            var senhas = _context.Senhas
                .Include(s => s.Prato)
                .Where(s => s.UsuarioId == user.Id)
                .OrderByDescending(s => s.DataEscolha)
                .ToList();

            return View(senhas);
        }
    }

}
