using AppQLHui.Data;
using AppQLHui.Models;
using AppQLHui.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ReportService _reportService;

        public ReportController(AppDbContext db, ReportService reportService)
        {
            _db = db;
            _reportService = reportService;
        }

        /// <summary>Sổ hụi - báo cáo trạng thái Âm/Dương tất cả hụi viên</summary>
        public async Task<IActionResult> Index()
        {
            var balances = await _reportService.GetPlayerNetBalanceAsync();
            return View(balances);
        }

        /// <summary>Bill đóng hụi theo ngày - tổng hợp theo ngày cho từng hụi viên</summary>
        public async Task<IActionResult> DailyBill(DateTime? date)
        {
            date ??= DateTime.Today;
            ViewBag.Date = date.Value;

            var transactions = await _db.Transactions
                .Include(t => t.Player)
                .Include(t => t.Draw).ThenInclude(d => d.Tontine)
                .Where(t => t.Draw.DrawDate.Date == date.Value.Date)
                .OrderBy(t => t.Player.Name)
                .ToListAsync();

            return View(transactions);
        }

        /// <summary>Giấy giao hụi - bill cho người hốt một kỳ cụ thể</summary>
        public async Task<IActionResult> WinnerBill(int drawId)
        {
            var draw = await _db.Draws
                .Include(d => d.Tontine)
                .Include(d => d.WinningShare).ThenInclude(ws => ws!.Player)
                .Include(d => d.Transactions).ThenInclude(t => t.Player)
                .FirstOrDefaultAsync(d => d.Id == drawId);
            if (draw == null) return NotFound();
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
    }
}
