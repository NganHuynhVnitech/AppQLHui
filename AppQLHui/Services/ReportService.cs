using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppQLHui.Data;
using AppQLHui.DTOs;
using AppQLHui.Models;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Services
{
    public class ReportService
    {
        private readonly AppDbContext _db;

        public ReportService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Tính trạng thái Âm/Dương của tất cả hụi viên trên toàn bộ dây hụi đang chạy.
        /// NetBalance = TotalLivingAmount - TotalDeadAmount
        ///   Dương (xanh): còn nợ chủ hụi (đóng nhiều hơn đã rút)
        ///   Âm   (đỏ):   đã rút về nhiều hơn đóng vào
        /// </summary>
        public async Task<List<PlayerNetBalanceDto>> GetPlayerNetBalanceAsync()
        {
            var players = await _db.Players
                .Include(p => p.TontineShares)
                    .ThenInclude(ts => ts.Tontine)
                .Include(p => p.Transactions)
                    .ThenInclude(tx => tx.Draw)
                .ToListAsync();

            var result = new List<PlayerNetBalanceDto>();

            foreach (var player in players)
            {
                // Tổng tiền hụi sống = số phần sống * mệnh giá tương ứng mỗi dây
                decimal totalLiving = player.TontineShares
                    .Where(ts => ts.Status == ShareStatus.Living)
                    .Sum(ts => ts.Tontine.BaseAmount);

                // Tổng tiền hụi chết = số phần chết * mệnh giá tương ứng mỗi dây
                decimal totalDead = player.TontineShares
                    .Where(ts => ts.Status == ShareStatus.Dead)
                    .Sum(ts => ts.Tontine.BaseAmount);

                // Tổng tiền thảo đã nộp = tổng CommissionFee của các draw mà player đã trúng
                decimal totalFees = player.Transactions
                    .Where(tx => tx.AmountReceive > 0)
                    .Sum(tx => tx.Draw.CollectedFee);

                result.Add(new PlayerNetBalanceDto
                {
                    PlayerId = player.Id,
                    PlayerName = player.Name,
                    Phone = player.Phone,
                    BankName = player.BankName,
                    BankAccountNumber = player.BankAccountNumber,
                    BankAccountName = player.BankAccountName,
                    TotalLivingAmount = totalLiving,
                    TotalDeadAmount = totalDead,
                    TotalFeesPaid = totalFees,
                    NetBalance = totalLiving - totalDead
                });
            }

            return result.OrderBy(r => r.PlayerName).ToList();
        }

        /// <summary>Lấy lịch sử giao dịch của một kỳ khui (dùng in hóa đơn)</summary>
        public async Task<List<Transaction>> GetDrawTransactionsAsync(int drawId)
        {
            return await _db.Transactions
                .Include(t => t.Player)
                .Include(t => t.Draw)
                    .ThenInclude(d => d.Tontine)
                .Where(t => t.DrawId == drawId)
                .ToListAsync();
        }

        /// <summary>Thống kê Dashboard</summary>
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            int runningCount = await _db.Tontines.CountAsync(t => t.Status == TontineStatus.Running);

            // Use in-memory aggregation (SQLite doesn't support Sum on decimal)
            var monthDraws = await _db.Draws
                .Where(d => d.DrawDate >= startOfMonth && d.DrawDate <= now)
                .ToListAsync();
            decimal monthlyFees = monthDraws.Sum(d => d.CollectedFee);

            var todayDraws = await _db.Draws
                .Include(d => d.Tontine)
                .Where(d => d.DrawDate.Date == now.Date)
                .ToListAsync();

            int totalPlayers = await _db.Players.CountAsync();
            decimal totalFees = await _db.Draws.SumAsync(d => d.CollectedFee);
            
            // Dây hụi đến hạn khui hôm nay (Running + chưa có draw hôm nay)
            var dueTodayTontines = await _db.Tontines
                .Where(t => t.Status == TontineStatus.Running)
                .ToListAsync();

            decimal activeCapital = await _db.Tontines
                .Where(t => t.Status == TontineStatus.Running)
                .SumAsync(t => t.BaseAmount * t.TotalShares);

            return new DashboardStatsDto
            {
                RunningTontineCount = runningCount,
                MonthlyFeesCollected = monthlyFees,
                TotalFeesCollected = totalFees,
                ActiveTontineCapital = activeCapital,
                TotalPlayers = totalPlayers,
                TodayDraws = todayDraws,
                DueTodayTontines = dueTodayTontines
            };
        }
        public async Task<DailySettlementDto> GetDailySettlementAsync(DateTime date)
        {
            var dayTransactions = await _db.Transactions
                .Include(t => t.Player)
                .Include(t => t.Draw)
                    .ThenInclude(d => d.Tontine)
                .Where(t => t.Draw.DrawDate.Date == date.Date)
                .ToListAsync();

            var playerTotals = dayTransactions
                .GroupBy(t => t.PlayerId)
                .Select(g => new PlayerDailyTotalDto
                {
                    PlayerId = g.Key,
                    PlayerName = g.First().Player.Name,
                    TotalToPay = g.Sum(t => t.AmountPay),
                    TotalToReceive = g.Sum(t => t.AmountReceive),
                    Details = g.Select(t => new TransactionDetailDto
                    {
                        TontineName = t.Draw.Tontine.Name,
                        SequenceNumber = t.Draw.SequenceNumber,
                        Amount = t.AmountReceive > 0 ? t.AmountReceive : -t.AmountPay,
                        Type = t.AmountReceive > 0 ? "Receive" : "Pay"
                    }).ToList()
                }).ToList();

            return new DailySettlementDto
            {
                Date = date,
                PlayerTotals = playerTotals,
                GrandTotalCollect = playerTotals.Sum(p => p.TotalToPay),
                GrandTotalPayout = playerTotals.Sum(p => p.TotalToReceive)
            };
        }

        public async Task<PlayerDailyTotalDto?> GetPlayerConsolidatedBillAsync(int playerId, DateTime date)
        {
            var settlement = await GetDailySettlementAsync(date);
            return settlement.PlayerTotals.FirstOrDefault(p => p.PlayerId == playerId);
        }

        public async Task<List<UnpaidTontineDto>> GetUnpaidDebtsAsync()
        {
            var unsettledTransactions = await _db.Transactions
                .Include(t => t.Player)
                .Include(t => t.Draw).ThenInclude(d => d.Tontine)
                // Filter: NetTotal < 0 (owes admin) AND not settled
                .Where(t => t.NetTotal < 0 && !t.IsSettled)
                .OrderBy(t => t.Draw.DrawDate)
                .ToListAsync();

            var grouped = unsettledTransactions
                .GroupBy(t => t.Draw.Tontine)
                .Select(gTontine => new UnpaidTontineDto
                {
                    TontineId = gTontine.Key.Id,
                    TontineName = gTontine.Key.Name,
                    TotalUnpaidAmount = gTontine.Sum(t => Math.Abs(t.NetTotal) - t.PaidAmount),
                    UnpaidPlayers = gTontine
                        .GroupBy(t => t.Player)
                        .Select(gPlayer => new UnpaidPlayerDto
                        {
                            PlayerId = gPlayer.Key.Id,
                            PlayerName = gPlayer.Key.Name,
                            TotalOwed = gPlayer.Sum(t => Math.Abs(t.NetTotal) - t.PaidAmount),
                            Transactions = gPlayer.Select(t => new UnpaidTransactionItemDto
                            {
                                TransactionId = t.Id,
                                DrawId = t.DrawId,
                                SequenceNumber = t.Draw.SequenceNumber,
                                DrawDate = t.Draw.DrawDate,
                                TotalDebtAmount = Math.Abs(t.NetTotal),
                                PaidAmount = t.PaidAmount,
                                RemainingAmount = Math.Abs(t.NetTotal) - t.PaidAmount
                            }).ToList()
                        }).ToList()
                }).ToList();

            return grouped;
        }

        public async Task<(bool success, string message)> SettleTransactionAsync(int transactionId, decimal paidAmount)
        {
            var tx = await _db.Transactions.FindAsync(transactionId);
            if (tx == null) return (false, "Không tìm thấy giao dịch.");
            
            decimal totalDebt = Math.Abs(tx.NetTotal);
            tx.PaidAmount += paidAmount;
            
            if (tx.PaidAmount >= totalDebt)
            {
                tx.PaidAmount = totalDebt; // Cap it
                tx.IsSettled = true;
                await _db.SaveChangesAsync();
                return (true, "Đã thu đủ và gạch nợ thành công.");
            }
            
            await _db.SaveChangesAsync();
            return (true, $"Đã ghi nhận thu {paidAmount.ToString("N0")}đ. Tiền nợ còn lại: {(totalDebt - tx.PaidAmount).ToString("N0")}đ.");
        }
    }

    public class DashboardStatsDto
    {
        public int RunningTontineCount { get; set; }
        public decimal MonthlyFeesCollected { get; set; }
        public decimal TotalFeesCollected { get; set; }
        public decimal ActiveTontineCapital { get; set; }
        public int TotalPlayers { get; set; }
        public List<Draw> TodayDraws { get; set; } = new();
        public List<Tontine> DueTodayTontines { get; set; } = new();
    }

    public class UnpaidTontineDto
    {
        public int TontineId { get; set; }
        public string TontineName { get; set; } = "";
        public decimal TotalUnpaidAmount { get; set; }
        public List<UnpaidPlayerDto> UnpaidPlayers { get; set; } = new();
    }

    public class UnpaidPlayerDto
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; } = "";
        public decimal TotalOwed { get; set; }
        public List<UnpaidTransactionItemDto> Transactions { get; set; } = new();
    }

    public class UnpaidTransactionItemDto
    {
        public int TransactionId { get; set; }
        public int DrawId { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime DrawDate { get; set; }
        public decimal TotalDebtAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
