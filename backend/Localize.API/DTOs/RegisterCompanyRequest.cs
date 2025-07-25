using System.ComponentModel.DataAnnotations;

namespace Localize.API.DTOs
{
    public class RegisterCompanyRequest
    {
        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter 14 dígitos.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter apenas dígitos.")]
        public string Cnpj { get; set; } = string.Empty;
    }
}