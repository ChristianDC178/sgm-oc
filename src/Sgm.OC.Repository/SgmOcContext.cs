using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Sgm.OC.Repository
{
    public class SgmOcContext : DbContext
    {

        public SgmOcContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())).UseSqlServer(Sgm.OC.Framework.Settings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

    }
}
