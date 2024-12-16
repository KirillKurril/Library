using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Library.Persistance.Contexts;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Data Source=192.168.1.106,1433;Initial Catalog=Library;Persist Security Info=True;User ID=library-api;Password=1525;Trust Server Certificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}
