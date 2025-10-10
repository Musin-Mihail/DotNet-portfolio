using Microsoft.EntityFrameworkCore;

namespace DotNet_portfolio.Services
{
    public interface IDbErrorService
    {
        bool IsUniqueConstraintViolation(Exception? ex);
    }
}
