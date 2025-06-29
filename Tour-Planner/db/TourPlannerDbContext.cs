using Microsoft.EntityFrameworkCore;

namespace Tour_Planner.db
{
    public class TourPlannerDbContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options)
            : base(options)
        {
            Console.WriteLine("TourPlannerDbContext initialized.");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tour>().ToTable("Tours");
            modelBuilder.Entity<TourLog>().ToTable("TourLogs");
            Console.WriteLine("Model creating for Tours and TourLogs tables.");
        }
    }
}