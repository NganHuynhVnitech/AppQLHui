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

            // Dây hụi đến hạn khui hôm nay (Running + chưa có draw hôm nay)
            var dueTodayTontines = await _db.Tontines
                .Where(t => t.Status == TontineStatus.Running)
                .ToListAsync();

            return new DashboardStatsDto
            {
                RunningTontineCount = runningCount,
                MonthlyFeesCollected = monthlyFees,
                TodayDraws = todayDraws,
                DueTodayTontines = dueTodayTontines
            };
        }
    }

    public class DashboardStatsDto
    {
        public int RunningTontineCount { get; set; }
        public decimal MonthlyFeesCollected { get; set; }
        public List<Draw> TodayDraws { get; set; } = new();
        public List<Tontine> DueTodayTontines { get; set; } = new();
    }
}
