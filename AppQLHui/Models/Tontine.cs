using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppQLHui.Models
{
    public enum TontineType { Day, Week, Month }
    public enum FeeType { Before, After } // Thảo thu trước / Thảo thu sau
    public enum TontineStatus { Draft, Running, Completed }

    public class Tontine
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        [Display(Name = "Tên dây hụi")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Loại hụi")]
        public TontineType Type { get; set; } = TontineType.Day;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Mệnh giá (M)")]
        public decimal BaseAmount { get; set; }

        [Display(Name = "Tổng số phần (N)")]
        public int TotalShares { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền thảo (F)")]
        public decimal CommissionFee { get; set; }

        [Display(Name = "Cách thu thảo")]
        public FeeType FeeType { get; set; } = FeeType.After;

        [Display(Name = "Trạng thái")]
        public TontineStatus Status { get; set; } = TontineStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<TontineShare> Shares { get; set; } = new List<TontineShare>();
        public ICollection<Draw> Draws { get; set; } = new List<Draw>();
    }
}
