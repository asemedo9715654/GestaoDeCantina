using GestaoDeCantina.Data;
using GestaoDeCantina.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestaoDeCantina.Controllers
{
    [Authorize]
    public class SenhaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SenhaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Escolher()
        {
            var dataHoje = DateTime.Today;
            var pratos = _context.Pratos
                .Where(p => p.Dia == dataHoje)
                .ToList();

            ViewBag.DataEscolhida = dataHoje;
            return View(pratos);
        }

        [HttpPost]
        public IActionResult FiltrarPorData(DateTime dataEscolhida)
        {
            if (dataEscolhida.Date < DateTime.Today)
            {
                TempData["Error"] = "Não é possível escolher pratos de dias anteriores.";
                return RedirectToAction("Escolher");
            }

            var pratos = _context.Pratos
                .Where(p => p.Dia == dataEscolhida.Date)
                .ToList();

            ViewBag.DataEscolhida = dataEscolhida;
            return View("Escolher", pratos);
        }



        [HttpPost]
        public IActionResult ConfirmarEscolha(int pratoId)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (usuarioId == null)
            {
                TempData["Error"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Conta");
            }

            var senha = new Senha
            {
                UsuarioId = usuarioId,
                PratoId = pratoId,
                DataEscolha = DateTime.Now
            };

            _context.Senhas.Add(senha);
            _context.SaveChanges();

            TempData["Success"] = "Senha escolhida com sucesso.";
            return RedirectToAction("MinhaSenha");
        }

        public IActionResult MinhaSenha()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (usuarioId == null)
            {
                TempData["Error"] = "Usuário não autenticado.";
                return RedirectToAction("Login", "Conta");
            }

            var senhas = _context.Senhas
                .Include(s => s.Prato)
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.DataEscolha)
                .ToList();

            return View(senhas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var senha = _context.Senhas.FirstOrDefault(s => s.Id == id && s.UsuarioId == usuarioId);

            if (senha == null)
            {
                TempData["Error"] = "Senha não encontrada ou não pertence a você.";
                return RedirectToAction("MinhaSenha");
            }

            _context.Senhas.Remove(senha);
            _context.SaveChanges();

            TempData["Success"] = "Senha eliminada com sucesso.";
            return RedirectToAction("MinhaSenha");
        }

    }
}
