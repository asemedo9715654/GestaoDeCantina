using Microsoft.AspNetCore.Identity;

namespace GestaoDeCantina.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Senha> Senhas { get; set; } = new List<Senha>();
    }
}
