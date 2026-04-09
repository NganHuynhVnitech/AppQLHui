using System.ComponentModel.DataAnnotations;

namespace AppQLHui.Models
{
    public enum UserRole { Admin, Owner }

    public class AppUser
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.Owner;

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankAccountName { get; set; }

        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Tontine> Tontines { get; set; } = new List<Tontine>();
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
