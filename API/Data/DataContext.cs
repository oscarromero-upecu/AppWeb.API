
using API.Entidades;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options):base(options)
        {
        }

        public DbSet<Usuario> Usuarios {get;set;}
    }
}