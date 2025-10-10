using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DotNet_portfolio.Services
{
    public class NpgsqlErrorService : IDbErrorService
    {
        private const string UniqueViolation_SqlState = "23505";

        public bool IsUniqueConstraintViolation(Exception? ex)
        {
            return ex is NpgsqlException npgsqlEx && npgsqlEx.SqlState == UniqueViolation_SqlState;
        }
    }
}
