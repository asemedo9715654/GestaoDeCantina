using GestaoDeCantina.Data;
using GestaoDeCantina.Models;
using GestaoDeCantina.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoDeCantina.Controllers
{
    // Controllers/PratoController.cs
    [Authorize(Roles = "Admin")]
    public class PratoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PratoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var pratos = _context.Pratos
                .OrderByDescending(p => p.Dia)
                .Select(p => new
                {
                    Prato = p,
                    TotalEscolhas = _context.Senhas.Count(s => s.PratoId == p.Id)
                })
                .ToList()
                .Select(x => new PratoViewModel
                {
                    Id = x.Prato.Id,
                    Nome = x.Prato.Nome,
                    Preco = x.Prato.Preco,
                    Dia = x.Prato.Dia,
                    TotalEscolhas = x.TotalEscolhas
                });

            return View(pratos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Criar(Prato prato)
        {
            if (ModelState.IsValid)
            {
                _context.Pratos.Add(prato);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(prato);
        }

        public IActionResult Editar(int id)
        {
            var prato = _context.Pratos.Find(id);
            if (prato == null)
            {
                return NotFound();
            }
            return View(prato);
        }

        [HttpPost]
        public IActionResult Editar(Prato prato)
        {
            if (ModelState.IsValid)
            {
                _context.Pratos.Update(prato);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(prato);
        }

        public IActionResult Eliminar(int id)
        {
            var prato = _context.Pratos.Find(id);
            if (prato == null)
            {
                return NotFound();
            }
            return View(prato);
        }

        [HttpPost, ActionName("Eliminar")]
        public IActionResult ConfirmarEliminar(int id)
        {
            var prato = _context.Pratos.Find(id);
            if (prato == null)
            {
                return NotFound();
            }

            _context.Pratos.Remove(prato);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }

}
