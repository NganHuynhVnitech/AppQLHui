using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppQLHui.Models
{
    public class Draw
    {
        public int Id { get; set; }

        public int TontineId { get; set; }
        public Tontine Tontine { get; set; } = null!;

        [Display(Name = "Kỳ số")]
        public int SequenceNumber { get; set; }

        [Display(Name = "Ngày khui")]
        public DateTime DrawDate { get; set; } = DateTime.Now;

        /// <summary>Phần hụi trúng kỳ này</summary>
        public int? WinningShareId { get; set; }
        public TontineShare? WinningShare { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền thăm kêu (B)")]
        public decimal BidAmount { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Tiền thảo thực tế thu")]
        public decimal CollectedFee { get; set; }

        // Navigation
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        /// <summary>Back-ref: shares whose WonDrawId points here</summary>
        public ICollection<TontineShare> WonShares { get; set; } = new List<TontineShare>();
    }
}
