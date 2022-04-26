
using Microsoft.EntityFrameworkCore;
using BurgersPizzas.Models;
namespace BurgersPizzas.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        //public DbSet<Parks> Parks { get; set; }
        //public DbSet<Park> Park { get; set; }

        public DbSet<BurgerDb> BurgersDb { get; set; }

        public DbSet<PizzaDb> PizzasDb { get; set; }
        public DbSet<CategoryDb> CategoryDb { get; set; }
    }
}


