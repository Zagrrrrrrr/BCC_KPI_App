using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BCC_KPI_App.Models
{
    public class SalesContext : DbContext
    {
        public SalesContext() : base("Server=ZAGR\\SQLEXPRESS;Database=SalesEfficiency_DB;Trusted_Connection=True;")
        {
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<KpiActual> KpiActuals { get; set; }
        public DbSet<KpiTarget> KpiTargets { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<UnitStatus> UnitStatuses { get; set; }
        public DbSet<Holding> Holdings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Unit>().ToTable("Units");
            modelBuilder.Entity<KpiTarget>().ToTable("KPI_Targets");
            modelBuilder.Entity<KpiActual>().ToTable("KPI_Actuals");
            modelBuilder.Entity<UnitType>().ToTable("UnitTypes");
            modelBuilder.Entity<UnitStatus>().ToTable("UnitStatuses");
            modelBuilder.Entity<Holding>().ToTable("Holdings");
        }
    }
}