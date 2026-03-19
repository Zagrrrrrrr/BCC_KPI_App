using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BCC_KPI_App.Models
{
    public class SalesContext : DbContext
    {
        // Жестко прописываем строку подключения прямо здесь
        public SalesContext() : base(GetConnectionString())
        {
        }

        private static string GetConnectionString()
        {
            return @"Server=ZAGR\SQLEXPRESS;Database=SalesEfficiency_DB;Trusted_Connection=True;TrustServerCertificate=True;";
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
        }
    }
}