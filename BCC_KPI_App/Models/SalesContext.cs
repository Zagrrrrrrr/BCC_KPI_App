using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BCC_KPI_App.Models
{
    public class SalesContext : DbContext
    {
        public SalesContext() : base("name=SalesEfficiencyDB")
        {
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<KpiActual> KpiActuals { get; set; }
        public DbSet<KpiTarget> KpiTargets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Unit>().ToTable("Units");
            modelBuilder.Entity<KpiTarget>().ToTable("KPI_Targets");
            modelBuilder.Entity<KpiActual>().ToTable("KPI_Actuals");

            // Указываем только те поля, которые есть в модели
            modelBuilder.Entity<Unit>().Ignore(u => u.KpiTargets);
            modelBuilder.Entity<Unit>().Ignore(u => u.KpiActuals);
        }
    }
}