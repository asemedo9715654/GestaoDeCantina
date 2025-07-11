using System.ComponentModel.DataAnnotations;

namespace GestaoDeCantina.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter ao menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Senha")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}
