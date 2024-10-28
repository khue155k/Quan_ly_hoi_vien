using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<MembershipFee> MembershipFees { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MembershipFee>()
                .HasOne(m => m.Company)
                .WithMany()
                .HasForeignKey(m => m.company_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MembershipFee>()
                .HasOne(m => m.Fee)
                .WithMany()
                .HasForeignKey(m => m.fee_id)
                .OnDelete(DeleteBehavior.Cascade);
            //modelBuilder.Entity<Company>().ToTable("Company");
        }
    }
}