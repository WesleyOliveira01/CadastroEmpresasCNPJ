using Xunit;
using Localize.API.Services;

namespace Localize.API.UnitTests
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService();
        }

        [Fact]
        public void HashPassword_ShouldReturnNonNullOrEmptyHash()
        {
            string password = "minhasenhaforte123";
            string hashedPassword = _authService.HashPassword(password);
            Assert.False(string.IsNullOrEmpty(hashedPassword));
            Assert.NotEqual(password, hashedPassword);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrueForCorrectPassword()
        {
            string password = "minhasenhaforte123";
            string hashedPassword = _authService.HashPassword(password);
            bool isPasswordCorrect = _authService.VerifyPassword(password, hashedPassword);
            Assert.True(isPasswordCorrect);
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalseForIncorrectPassword()
        {
            string correctPassword = "minhasenhaforte123";
            string incorrectPassword = "senhaincorreta";
            string hashedPassword = _authService.HashPassword(correctPassword);
            bool isPasswordCorrect = _authService.VerifyPassword(incorrectPassword, hashedPassword);
            Assert.False(isPasswordCorrect);
        }

        [Fact]
        public void VerifyPassword_ShouldThrowSaltParseExceptionForInvalidHashFormat()
        {
            string password = "minhasenhaforte123";
            string invalidHashFormat = "thisisnotavalidbcrypthash";
            var exception = Record.Exception(() => _authService.VerifyPassword(password, invalidHashFormat));
            Assert.NotNull(exception);
            Assert.IsType<BCrypt.Net.SaltParseException>(exception);
            Assert.Contains("Invalid salt version", exception.Message);
        }
    }
}