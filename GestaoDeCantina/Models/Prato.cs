namespace GestaoDeCantina.Models
{
    public class Prato
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Preco { get; set; }
        public DateTime Dia { get; set; }
        public virtual ICollection<Senha>? Senhas { get; set; }
    }
}
