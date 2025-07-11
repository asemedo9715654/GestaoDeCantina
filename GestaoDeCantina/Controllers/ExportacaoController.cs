using ClosedXML.Excel;
using GestaoDeCantina.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace GestaoDeCantina.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExportacaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExportacaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Excel()
        {
            var senhas = _context.Senhas
                .Include(s => s.Usuario)
                .Include(s => s.Prato)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Senhas");

            worksheet.Cell(1, 1).Value = "Usuário";
            worksheet.Cell(1, 2).Value = "Prato";
            worksheet.Cell(1, 3).Value = "Data da Escolha";

            int linha = 2;
            foreach (var senha in senhas)
            {
                worksheet.Cell(linha, 1).Value = senha.Usuario?.UserName ?? "N/A";
                worksheet.Cell(linha, 2).Value = senha.Prato?.Nome ?? "N/A";
                worksheet.Cell(linha, 3).Value = senha.DataEscolha.ToString("dd/MM/yyyy HH:mm");
                linha++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Senhas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        public IActionResult Pdf()
        {
            var senhas = _context.Senhas
                .Include(s => s.Usuario)
                .Include(s => s.Prato)
                .ToList();

            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(595, 842);
                    page.Margin(30);
                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text("Relatório de Senhas").FontSize(20).Bold().AlignCenter();
                            col.Item().LineHorizontal(1);

                            foreach (var senha in senhas)
                            {
                                col.Item().PaddingVertical(5).Text(
                                    $"{senha.Usuario?.UserName} escolheu '{senha.Prato?.Nome}' em {senha.DataEscolha:dd/MM/yyyy HH:mm}");
                            }
                        });
                });
            });

            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            return File(stream.ToArray(), "application/pdf", $"Senhas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }

    }

}
