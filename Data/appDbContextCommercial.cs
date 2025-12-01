using Microsoft.EntityFrameworkCore;
using ProyectoWebCommercialLopez.Models;

namespace ProyectoWebCommercialLopez.Data
{
    public class appDbContextCommercial: DbContext
    {
        public appDbContextCommercial(DbContextOptions<appDbContextCommercial> options)
            : base(options) { }

        public DbSet<Person> Person { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Product> Product { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Supplier > Supplier { get; set; }

        public DbSet<WareHouse> WareHouse { get; set; }
        public DbSet<PasswocrdResetToken> PasswordResetToken { get; set; }

    }
}
