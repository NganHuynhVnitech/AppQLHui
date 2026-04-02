using AppQLHui.Data;
using AppQLHui.Models;
using AppQLHui.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    public class DrawController : Controller
    {
        private readonly AppDbContext _db;
        private readonly TontineService _tontineService;
        private readonly DrawService _drawService;
        private readonly ReportService _reportService;

        public DrawController(AppDbContext db, TontineService tontineService,
            DrawService drawService, ReportService reportService)
        {
            _db = db;
            _tontineService = tontineService;
            _drawService = drawService;
            _reportService = reportService;
        }

        /// <summary>Màn hình khui hụi 2 panel</summary>
        public async Task<IActionResult> Index(int? tontineId)
        {
            var runningTontines = await _db.Tontines
                .Where(t => t.Status == TontineStatus.Running)
                .OrderBy(t => t.Name)
                .ToListAsync();
            ViewBag.RunningTontines = runningTontines;

            if (tontineId.HasValue)
            {
                var tontine = await _db.Tontines
                    .Include(t => t.Shares).ThenInclude(s => s.Player)
                    .FirstOrDefaultAsync(t => t.Id == tontineId.Value);
                ViewBag.SelectedTontine = tontine;

                var livingShares = tontine?.Shares
                    .Where(s => s.Status == ShareStatus.Living)
                    .OrderBy(s => s.Player.Name)
                    .ToList();
                ViewBag.LivingShares = livingShares;
            }

            return View();
        }

        /// <summary>AJAX: Tính toán preview trước khi chốt</summary>
        [HttpPost]
        public async Task<IActionResult> Preview(int tontineId, int winningShareId, decimal bidAmount)
        {
            try
            {
                var preview = await _tontineService.CalculateDrawPreviewAsync(tontineId, winningShareId, bidAmount);
                return Json(new { success = true, data = preview });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>Chốt kỳ khui hụi</summary>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Execute(int tontineId, int winningShareId, decimal bidAmount, DateTime drawDate)
        {
            try
            {
                var draw = await _drawService.ExecuteDrawAsync(tontineId, winningShareId, bidAmount, drawDate);
                TempData["Success"] = $"Đã chốt kỳ khui số {draw.SequenceNumber} thành công!";
                return RedirectToAction("Bill", new { drawId = draw.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { tontineId });
            }
        }

        /// <summary>In hóa đơn kỳ khui</summary>
        public async Task<IActionResult> Bill(int drawId)
        {
            var transactions = await _reportService.GetDrawTransactionsAsync(drawId);
            var draw = await _db.Draws
                .Include(d => d.Tontine)
                .Include(d => d.WinningShare).ThenInclude(ws => ws!.Player)
                .FirstOrDefaultAsync(d => d.Id == drawId);
            if (draw == null) return NotFound();
            ViewBag.Draw = draw;
            return View(transactions);
        }

        /// <summary>Lịch sử các kỳ khui của một dây hụi</summary>
        public async Task<IActionResult> History(int tontineId)
        {
            var tontine = await _db.Tontines.FindAsync(tontineId);
            if (tontine == null) return NotFound();
            ViewBag.Tontine = tontine;

            var draws = await _db.Draws
                .Include(d => d.WinningShare).ThenInclude(ws => ws!.Player)
                .Include(d => d.Transactions).ThenInclude(t => t.Player)
                .Where(d => d.TontineId == tontineId)
                .OrderBy(d => d.SequenceNumber)
                .ToListAsync();
            return View(draws);
        }

        /// <summary>API: Quay số ngẫu nhiên (có thể override target cho Admin)</summary>
        [HttpGet]
        public async Task<IActionResult> RandomDraw(int tontineId, int? targetPlayerId)
        {
            var livingShares = await _tontineService.GetLivingSharesAsync(tontineId);
            if (!livingShares.Any())
                return Json(new { success = false, message = "Không còn phần sống nào." });

            TontineShare chosen;
            if (targetPlayerId.HasValue)
            {
                var targetShares = livingShares.Where(s => s.PlayerId == targetPlayerId.Value).ToList();
                chosen = targetShares.Any()
                    ? targetShares[Random.Shared.Next(targetShares.Count)]
                    : livingShares[Random.Shared.Next(livingShares.Count)];
            }
            else
            {
                chosen = livingShares[Random.Shared.Next(livingShares.Count)];
            }

            return Json(new
            {
                success = true,
                shareId = chosen.Id,
                playerName = chosen.Player.Name,
                playerId = chosen.PlayerId
            });
        }
    }
}
