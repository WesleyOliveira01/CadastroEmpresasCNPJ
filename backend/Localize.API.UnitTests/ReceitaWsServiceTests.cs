using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Localize.API.Services;
using Localize.API.ExternalApiModels;

namespace Localize.API.UnitTests
{
    public class ReceitaWsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly ReceitaWsService _receitaWsService;

        public ReceitaWsServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _receitaWsService = new ReceitaWsService(_httpClient);
        }

        private void SetupMockResponse(HttpStatusCode statusCode, string content)
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json"),
                    RequestMessage = new HttpRequestMessage()
                });
        }

        [Fact]
        public async Task GetCompanyDataByCnpjAsync_ShouldReturnCompanyData_WhenApiReturnsSuccess()
        {
            var cnpj = "00000000000100";
            var expectedCompanyResponse = new ReceitaWsCompanyResponse 
            { 
                Status = "OK", 
                Cnpj = cnpj, 
                NomeEmpresarial = "EMPRESA TESTE LTDA",
                NomeFantasia = "TESTE",
                AtividadePrincipal = new List<Activity> { new Activity { Code = "1234-5/00", Text = "Atividade Principal Teste" } }
            };
            var jsonResponse = JsonSerializer.Serialize(expectedCompanyResponse);
            SetupMockResponse(HttpStatusCode.OK, jsonResponse);

            var result = await _receitaWsService.GetCompanyDataByCnpjAsync(cnpj);

            Assert.NotNull(result);
            Assert.Equal(expectedCompanyResponse.Cnpj, result.Cnpj);
            Assert.Equal(expectedCompanyResponse.NomeEmpresarial, result.NomeEmpresarial);
            Assert.Equal(expectedCompanyResponse.NomeFantasia, result.NomeFantasia);
            Assert.Equal("OK", result.Status);
            Assert.NotNull(result.AtividadePrincipal);
            Assert.NotEmpty(result.AtividadePrincipal);
            Assert.Equal("1234-5/00", result.AtividadePrincipal[0].Code);
        }

        [Fact]
        public async Task GetCompanyDataByCnpjAsync_ShouldReturnNull_WhenApiReturnsErrorStatus()
        {
            var cnpj = "00000000000100";
            var errorCompanyResponse = new ReceitaWsCompanyResponse { Status = "ERROR", Message = "CNPJ inválido" };
            var jsonResponse = JsonSerializer.Serialize(errorCompanyResponse);
            SetupMockResponse(HttpStatusCode.OK, jsonResponse);

            var result = await _receitaWsService.GetCompanyDataByCnpjAsync(cnpj);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("123")] 
        [InlineData("123456789012345")]
        [InlineData("abcde123456789")]
        [InlineData((string)null)]
        [InlineData("")] 
        [InlineData(" ")] 
        public async Task GetCompanyDataByCnpjAsync_ShouldReturnNull_WhenCnpjIsInvalidFormat(string? cnpj)
        {
            var result = await _receitaWsService.GetCompanyDataByCnpjAsync(cnpj);

            Assert.Null(result);
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetCompanyDataByCnpjAsync_ShouldReturnNull_WhenApiReturnsNonSuccessStatusCode()
        {
            var cnpj = "00000000000100";
            SetupMockResponse(HttpStatusCode.NotFound, "{\"message\":\"CNPJ not found\"}"); 

            var result = await _receitaWsService.GetCompanyDataByCnpjAsync(cnpj);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCompanyDataByCnpjAsync_ShouldReturnNull_WhenApiReturnsMalformedJson()
        {
            var cnpj = "00000000000100";
            var malformedJson = "{ \"status\": \"OK\", \"nome_empresarial\": \"EMPRESA TESTE\", \"cnpj\": \"12345678901234\", }";
            SetupMockResponse(HttpStatusCode.OK, malformedJson);

            var result = await _receitaWsService.GetCompanyDataByCnpjAsync(cnpj);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCompanyDataByCnpjAsync_ShouldReturnNull_WhenHttpRequestExceptionOccurs()
        {
            var cnpj = "00000000000100";
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Simulated network error"));

            var result = await _receitaWsService.GetCompanyDataByCnpjAsync(cnpj);

            Assert.Null(result);
        }
    }
}