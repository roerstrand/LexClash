using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrdSpel.DAL.Data;

public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=ordspel;Integrated Security=True;" +
            "TrustServerCertificate=True;MultipleActiveResultSets=True;");
        return new AuthDbContext(optionsBuilder.Options);
    }
}
