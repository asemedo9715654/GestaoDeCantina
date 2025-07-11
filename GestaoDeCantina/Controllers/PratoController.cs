using GestaoDeCantina.Data;
using GestaoDeCantina.Models;
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
                .ToList();
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
    }

}
