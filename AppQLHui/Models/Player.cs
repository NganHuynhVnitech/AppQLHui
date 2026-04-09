using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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

        [MaxLength(50)]
        [Display(Name = "Ngân hàng")]
        public string? BankName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Số tài khoản")]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        [Display(Name = "Chủ tài khoản")]
        public string? BankAccountName { get; set; }

        [MaxLength(255)]
        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Ownership
        public int OwnerId { get; set; }
        [ValidateNever]
        public AppUser Owner { get; set; } = null!;

        // Navigation
        [ValidateNever]
        public ICollection<TontineShare> TontineShares { get; set; } = new List<TontineShare>();
        [ValidateNever]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
