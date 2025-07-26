using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Localize.API.Services;
using Localize.API.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Localize.API.UnitTests
{
    public class JwtServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockJwtSection;
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockJwtSection = new Mock<IConfigurationSection>();

            _mockConfiguration.Setup(c => c.GetSection("Jwt")).Returns(_mockJwtSection.Object);

            _mockJwtSection.Setup(s => s["Key"]).Returns("chave_secreta_muito_longa_e_segura_para_teste_com_no_minimo_16_caracteres"); 
            _mockJwtSection.Setup(s => s["ExpiryMinutes"]).Returns("60");
            _mockJwtSection.Setup(s => s["Issuer"]).Returns("Localize.API");
            _mockJwtSection.Setup(s => s["Audience"]).Returns("Localize.Web");

            _jwtService = new JwtService(_mockConfiguration.Object);
        }

        [Fact]
        public void GenerateToken_ShouldReturnValidToken()
        {
            var testUser = new User
            {
                Id = 1,
                Email = "teste@example.com",
                Name = "Usuário Teste"
            };

            var token = _jwtService.GenerateToken(testUser);

            Assert.False(string.IsNullOrEmpty(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _mockConfiguration.Object.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = false
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                Assert.Equal(testUser.Id.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                Assert.Equal(testUser.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
                Assert.Equal(testUser.Name, principal.FindFirst(ClaimTypes.Name)?.Value);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Token validation failed: {ex.Message}");
            }
        }
    }
}