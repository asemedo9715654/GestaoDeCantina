namespace GestaoDeCantina.Models
{
    public class Senha
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public ApplicationUser? Usuario { get; set; }

        public int PratoId { get; set; }
        public Prato? Prato { get; set; }

        public DateTime DataEscolha { get; set; } = DateTime.Now;
    }
}
