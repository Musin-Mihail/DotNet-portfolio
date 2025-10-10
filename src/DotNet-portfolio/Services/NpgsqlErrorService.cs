using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DotNet_portfolio.Services
{
    public class NpgsqlErrorService : IDbErrorService
    {
        private const string UniqueViolation_SqlState = "23505";

        public bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is PostgresException postgresEx
                && postgresEx.SqlState == UniqueViolation_SqlState;
        }
    }
}
