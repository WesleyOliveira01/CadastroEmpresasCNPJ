using System.Text.Json.Serialization;

namespace Localize.API.ExternalApiModels
{
    public class ReceitaWsCompanyResponse
    {
        [JsonPropertyName("nome")]
        public string NomeEmpresarial { get; set; } = string.Empty;

        [JsonPropertyName("fantasia")]
        public string NomeFantasia { get; set; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("situacao")]
        public string Situacao { get; set; } = string.Empty;

        [JsonPropertyName("abertura")]
        public string Abertura { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("natureza_juridica")]
        public string NaturezaJuridica { get; set; } = string.Empty;

        [JsonPropertyName("atividade_principal")]
        public List<Activity>? AtividadePrincipal { get; set; }

        [JsonPropertyName("logradouro")]
        public string Logradouro { get; set; } = string.Empty;

        [JsonPropertyName("numero")]
        public string Numero { get; set; } = string.Empty;

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string Bairro { get; set; } = string.Empty;

        [JsonPropertyName("municipio")]
        public string Municipio { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string Uf { get; set; } = string.Empty;

        [JsonPropertyName("cep")]
        public string Cep { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    public class Activity
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}