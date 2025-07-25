using System.Net.Http;
using System.Text.Json;
using Localize.API.ExternalApiModels;

namespace Localize.API.Services
{
    public class ReceitaWsService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.receitaws.com.br/v1/cnpj/";

        public ReceitaWsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ReceitaWsCompanyResponse?> GetCompanyDataByCnpjAsync(string cnpj)
        {
            cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
            {
                return null; 
            }

            var requestUrl = $"{BaseUrl}{cnpj}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var companyData = JsonSerializer.Deserialize<ReceitaWsCompanyResponse>(jsonString, options);
                    if (companyData != null && companyData.Status?.ToUpper() == "ERROR")
                    {
                        return null;
                    }

                    return companyData;
                }
                else
                {
                    Console.WriteLine($"Erro ao consultar CNPJ {cnpj}: Status Code {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erro de requisição HTTP para CNPJ {cnpj}: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Erro de desserialização JSON para CNPJ {cnpj}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado ao consultar CNPJ {cnpj}: {ex.Message}");
                return null;
            }
        }
    }
}