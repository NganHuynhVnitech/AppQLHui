using AppQLHui.Models;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Tontine> Tontines { get; set; }
        public DbSet<TontineShare> TontineShares { get; set; }
        public DbSet<Draw> Draws { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TontineShare → Tontine
            modelBuilder.Entity<TontineShare>()
                .HasOne(ts => ts.Tontine)
                .WithMany(t => t.Shares)
                .HasForeignKey(ts => ts.TontineId)
                .OnDelete(DeleteBehavior.Cascade);

            // TontineShare → Player
            modelBuilder.Entity<TontineShare>()
                .HasOne(ts => ts.Player)
                .WithMany(p => p.TontineShares)
                .HasForeignKey(ts => ts.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TontineShare → WonDraw (phần đã hốt)
            modelBuilder.Entity<TontineShare>()
                .HasOne(ts => ts.WonDraw)
                .WithMany(d => d.WonShares)
                .HasForeignKey(ts => ts.WonDrawId)
                .OnDelete(DeleteBehavior.SetNull);

            // Draw → Tontine
            modelBuilder.Entity<Draw>()
                .HasOne(d => d.Tontine)
                .WithMany(t => t.Draws)
                .HasForeignKey(d => d.TontineId)
                .OnDelete(DeleteBehavior.Cascade);

            // Draw → WinningShare (nullable)
            modelBuilder.Entity<Draw>()
                .HasOne(d => d.WinningShare)
                .WithMany()
                .HasForeignKey(d => d.WinningShareId)
                .OnDelete(DeleteBehavior.SetNull);

            // Transaction → Draw
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Draw)
                .WithMany(d => d.Transactions)
                .HasForeignKey(t => t.DrawId)
                .OnDelete(DeleteBehavior.Cascade);

            // Transaction → Player
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Player)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
