using DotNet_portfolio.Services;
using Moq;
using Npgsql;

namespace DotNet_portfolio.Tests
{
    public class NpgsqlErrorServiceTests
    {
        [Fact]
        public void IsUniqueConstraintViolation_ReturnsTrue_WhenExceptionIsNpgsqlAndSqlStateIsCorrect()
        {
            var service = new NpgsqlErrorService();
            var mockException = new Mock<NpgsqlException>();
            mockException.SetupGet(e => e.SqlState).Returns("23505");

            var result = service.IsUniqueConstraintViolation(mockException.Object);

            Assert.True(result);
        }

        [Fact]
        public void IsUniqueConstraintViolation_ReturnsFalse_WhenSqlStateIsIncorrect()
        {
            var service = new NpgsqlErrorService();
            var mockException = new Mock<NpgsqlException>();
            mockException.SetupGet(e => e.SqlState).Returns("XXXXX");

            var result = service.IsUniqueConstraintViolation(mockException.Object);

            Assert.False(result);
        }

        [Fact]
        public void IsUniqueConstraintViolation_ReturnsFalse_WhenExceptionIsNotNpgsqlException()
        {
            var service = new NpgsqlErrorService();
            var genericException = new Exception("Generic error");

            var result = service.IsUniqueConstraintViolation(genericException);

            Assert.False(result);
        }

        [Fact]
        public void IsUniqueConstraintViolation_ReturnsFalse_WhenExceptionIsNull()
        {
            var service = new NpgsqlErrorService();

            var result = service.IsUniqueConstraintViolation(null);

            Assert.False(result);
        }
    }
}
