using AppQLHui.Data;
using AppQLHui.Models;
using AppQLHui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ReportService _reportService;

        public ReportController(AppDbContext db, ReportService reportService)
        {
            _db = db;
            _reportService = reportService;
        }

        private async Task<AppUser?> GetCurrentOwnerAsync()
        {
            if (_db.CurrentUserId == null) return null;
            return await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _db.CurrentUserId);
        }

        /// <summary>Sổ hụi - báo cáo trạng thái Âm/Dương tất cả hụi viên</summary>
        public async Task<IActionResult> Index()
        {
            var balances = await _reportService.GetPlayerNetBalanceAsync();
            return View(balances);
        }

        /// <summary>Báo cáo gom hụi hàng ngày - Danh sách tất cả người chơi có giao dịch</summary>
        public async Task<IActionResult> DailySettlement(DateTime? date)
        {
            date ??= DateTime.Today;
            var settlement = await _reportService.GetDailySettlementAsync(date.Value);
            ViewBag.Owner = await GetCurrentOwnerAsync();
            return View(settlement);
        }

        /// <summary>Bill gom hụi chi tiết theo từng người chơi trong ngày</summary>
        public async Task<IActionResult> DailyBill(DateTime? date)
        {
            date ??= DateTime.Today;
            var transactions = await _db.Transactions
                .Include(t => t.Player)
                .Include(t => t.Draw).ThenInclude(d => d.Tontine)
                .Where(t => t.Draw.DrawDate.Date == date.Value.Date)
                .ToListAsync();

            ViewBag.Date = date.Value;
            ViewBag.Owner = await GetCurrentOwnerAsync();
            
            return View(transactions);
        }

        /// <summary>Bill hốt/đóng hụi tổng hợp cho một người chơi trong ngày (Consolidated Bill)</summary>
        public async Task<IActionResult> IndividualSummary(int playerId, DateTime? date)
        {
            date ??= DateTime.Today;
            var playerBill = await _reportService.GetPlayerConsolidatedBillAsync(playerId, date.Value);
            if (playerBill == null) return NotFound();
            
            var player = await _db.Players.FindAsync(playerId);
            ViewBag.Player = player;
            ViewBag.Date = date.Value;
            ViewBag.Owner = await GetCurrentOwnerAsync();

            return View(playerBill);
        }

        /// <summary>Giấy giao hụi - bill cho người hốt một kỳ cụ thể</summary>
        public async Task<IActionResult> WinnerBill(int drawId)
        {
            var draw = await _db.Draws
                .Include(d => d.Tontine).ThenInclude(t => t.Owner)
                .Include(d => d.WinningShare).ThenInclude(ws => ws!.Player)
                .Include(d => d.Transactions).ThenInclude(t => t.Player)
                .FirstOrDefaultAsync(d => d.Id == drawId);
            if (draw == null) return NotFound();
            ViewBag.Owner = await GetCurrentOwnerAsync();
            return View(draw);
        }

        /// <summary>Lịch sử giao dịch của một hụi viên</summary>
        public async Task<IActionResult> PlayerHistory(int playerId)
        {
            var player = await _db.Players.FindAsync(playerId);
            if (player == null) return NotFound();
            ViewBag.Player = player;

            var transactions = await _db.Transactions
                .Include(t => t.Draw).ThenInclude(d => d.Tontine)
                .Where(t => t.PlayerId == playerId)
                .OrderByDescending(t => t.Draw.DrawDate)
                .ToListAsync();
            return View(transactions);
        }

        /// <summary>Danh sách người chưa đóng hụi nợ tiền chủ hụi (Sổ Công Nợ)</summary>
        public async Task<IActionResult> UnpaidDebts()
        {
            var debts = await _reportService.GetUnpaidDebtsAsync();
            return View(debts);
        }

        /// <summary>API: Chốt thanh toán tiền (Đã thu xong nợ của một giao dịch)</summary>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSettlement(int transactionId, decimal amount)
        {
            try
            {
                var result = await _reportService.SettleTransactionAsync(transactionId, amount);
                return Json(new { success = result.success, message = result.message });
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkSettle(int[] transactionIds)
        {
            try
            {
                if (transactionIds == null || transactionIds.Length == 0)
                    return Json(new { success = false, message = "Không có hụi viên nào được chọn." });

                int count = 0;
                foreach (var txId in transactionIds)
                {
                    // For bulk settle, we fetch the transaction to get the remaining balance
                    var tx = await _db.Transactions.FindAsync(txId);
                    if (tx != null && !tx.IsSettled)
                    {
                        decimal remain = Math.Abs(tx.NetTotal) - tx.PaidAmount;
                        await _reportService.SettleTransactionAsync(txId, remain);
                        count++;
                    }
                }

                return Json(new { success = true, message = $"Đã thu tiền thành công cho {count} hụi viên." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
