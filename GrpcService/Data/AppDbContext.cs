using GrpcService.Model;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
