using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AppQLHui.Models
{
    public class Draw
    {
        public int Id { get; set; }

        public int TontineId { get; set; }
        [ValidateNever]
        public Tontine Tontine { get; set; } = null!;

        [Display(Name = "Kỳ số")]
        public int SequenceNumber { get; set; }

        [Display(Name = "Ngày khui")]
        public DateTime DrawDate { get; set; } = DateTime.Now;

        /// <summary>Phần hụi trúng kỳ này</summary>
        public int? WinningShareId { get; set; }
        [ValidateNever]
        public TontineShare? WinningShare { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền thăm kêu (B)")]
        public decimal BidAmount { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền thảo thực tế thu")]
        public decimal CollectedFee { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Trừ nợ cũ")]
        public decimal OldDebtDeduction { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Trừ khác")]
        public decimal OtherDeduction { get; set; }

        [Display(Name = "Ghi chú trừ khác")]
        public string? OtherDeductionNote { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Thực giao")]
        public decimal ActualReceived { get; set; }

        [Display(Name = "Số phần sống")]
        public int CountLivingShares { get; set; }

        [Display(Name = "Số phần chết")]
        public int CountDeadShares { get; set; }

        // Navigation
        [ValidateNever]
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        /// <summary>Back-ref: shares whose WonDrawId points here</summary>
        [ValidateNever]
        public ICollection<TontineShare> WonShares { get; set; } = new List<TontineShare>();
    }
}
