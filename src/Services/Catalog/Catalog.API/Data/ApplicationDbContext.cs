namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Plate> Plates { get; set; } = null!;
        public DbSet<PlateAuditLog> PlateAuditLogs { get; set; } = null!;
    }
}
