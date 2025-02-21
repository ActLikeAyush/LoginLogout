using Microsoft.EntityFrameworkCore;
using TestAPIadminPortal.Models.Entites;

namespace TestAPIadminPortal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
             
        }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
