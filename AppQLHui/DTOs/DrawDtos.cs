using System.Collections.Generic;

namespace AppQLHui.DTOs
{
    public class DrawPreviewDto
    {
        public int TontineId { get; set; }
        public string TontineName { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal BidAmount { get; set; }
        public decimal CommissionFee { get; set; }
        public int WinningShareId { get; set; }
        public int WinnerPlayerId { get; set; }
        public string WinnerPlayerName { get; set; } = string.Empty;
        
        // Math breakdown for "Giấy giao hụi"
        public int CountLivingPortions { get; set; }
        public int CountDeadPortions { get; set; }
        public decimal AmountPerLivingPortion => BaseAmount - BidAmount;
        public decimal AmountPerDeadPortion => BaseAmount;
        
        public decimal TotalFromLiving { get; set; }
        public decimal TotalFromDead { get; set; }
        public decimal TotalReceived { get; set; }

        // Professional Deductions
        public decimal OldDebtDeduction { get; set; }
        public decimal OtherDeduction { get; set; }
        public string? OtherDeductionNote { get; set; }
        public decimal FinalReceived => TotalReceived - OldDebtDeduction - OtherDeduction;

        public List<PlayerTransactionDto> PlayerTransactions { get; set; } = new();
    }

    public class PlayerTransactionDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? Phone { get; set; }

        /// <summary>Số phần sống mà player này đang giữ trong dây (chưa hốt, và không phải phần vừa trúng)</summary>
        public int LivingShareCount { get; set; }

        /// <summary>Số phần chết mà player này đang giữ trong dây</summary>
        public int DeadShareCount { get; set; }

        /// <summary>Tổng tiền phải đóng (trước bù trừ)</summary>
        public decimal AmountPay { get; set; }

        /// <summary>Tổng tiền được nhận (nếu là người trúng)</summary>
        public decimal AmountReceive { get; set; }

        /// <summary>Số tiền thực nhận/đóng sau khi bù trừ (+ = nhận, - = phải đóng)</summary>
        public decimal NetTotal { get; set; }

        public decimal OldDebtDeduction { get; set; }
        public decimal OtherDeduction { get; set; }
        public string? OtherDeductionNote { get; set; }
        public decimal FinalTotal => NetTotal - OldDebtDeduction - OtherDeduction;

        public bool IsWinner { get; set; }
    }

    public class PlayerNetBalanceDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }

        /// <summary>Tổng tiền hụi sống (chưa hốt)</summary>
        public decimal TotalLivingAmount { get; set; }

        /// <summary>Tổng tiền hụi chết (đã hốt)</summary>
        public decimal TotalDeadAmount { get; set; }

        /// <summary>Tổng tiền thảo đã nộp</summary>
        public decimal TotalFeesPaid { get; set; }

        /// <summary>Tiền Âm/Dương = Tổng hụi sống - Tổng hụi chết. Dương = đang nợ, Âm = đã thu về nhiều hơn</summary>
        public decimal NetBalance { get; set; }
    }

    public class DailySettlementDto
    {
        public DateTime Date { get; set; }
        public List<PlayerDailyTotalDto> PlayerTotals { get; set; } = new();
        public decimal GrandTotalCollect { get; set; }
        public decimal GrandTotalPayout { get; set; }
    }

    public class PlayerDailyTotalDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public decimal TotalToPay { get; set; }
        public decimal TotalToReceive { get; set; }
        public decimal NetBalance => TotalToReceive - TotalToPay;
        public List<TransactionDetailDto> Details { get; set; } = new();
    }

    public class TransactionDetailDto
    {
        public string TontineName { get; set; } = string.Empty;
        public int SequenceNumber { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } = string.Empty; // "Pay" or "Receive"
    }
}
