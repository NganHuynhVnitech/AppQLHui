using AppQLHui.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppQLHui.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) 
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<AppUser> Users { get; set; } = null!;
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Tontine> Tontines { get; set; } = null!;
        public DbSet<TontineShare> TontineShares { get; set; } = null!;
        public DbSet<Draw> Draws { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<ZaloSettings> ZaloSettings { get; set; } = null!;

        public int? CurrentUserId
        {
            get
            {
                var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(userIdStr, out var id) ? id : null;
            }
        }

        public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole(UserRole.Admin.ToString()) ?? false;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Query Filters for Multi-Tenancy
            modelBuilder.Entity<Tontine>().HasQueryFilter(t => IsAdmin || t.OwnerId == CurrentUserId);
            modelBuilder.Entity<Player>().HasQueryFilter(p => IsAdmin || p.OwnerId == CurrentUserId);
            modelBuilder.Entity<ZaloSettings>().HasQueryFilter(z => IsAdmin || z.OwnerId == CurrentUserId);

            // Relationships
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.Username).IsUnique();

            // TontineShare → Tontine
            modelBuilder.Entity<TontineShare>()
                .HasOne(ts => ts.Tontine)
                .WithMany(t => t.Shares)
                .HasForeignKey(ts => ts.TontineId)
                .OnDelete(DeleteBehavior.Restrict);

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
