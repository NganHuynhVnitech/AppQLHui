using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppQLHui.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "Tên hụi viên")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Display(Name = "Tên Zalo")]
        public string? ZaloName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<TontineShare> TontineShares { get; set; } = new List<TontineShare>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
