using GestaoDeCantina.Data;
using GestaoDeCantina.Models;
using System.Security.Cryptography;
using System.Text;

namespace GestaoDeCantina.Service
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Usuario ValidarUsuario(string email, string senha)
        {
            var senhaHash = HashSenha(senha);
            return _context.Usuarios.FirstOrDefault(u => u.Email == email && u.SenhaHash == senhaHash);
        }

        public static string HashSenha(string senha)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(senha);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
