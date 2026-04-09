using AppQLHui.Data;
using AppQLHui.Models;
using AppQLHui.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    [Authorize]
    public class TontineController : Controller
    {
        private readonly AppDbContext _db;
        private readonly DrawService _drawService;

        public TontineController(AppDbContext db, DrawService drawService)
        {
            _db = db;
            _drawService = drawService;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            filter ??= "enable";
            var query = _db.Tontines.Include(t => t.Shares).AsQueryable();

            if (filter == "enable")
            {
                query = query.Where(t => t.Status == TontineStatus.Draft || t.Status == TontineStatus.Running);
            }
            else if (filter == "disable")
            {
                query = query.Where(t => t.Status == TontineStatus.Completed || t.Status == TontineStatus.Disabled);
            }

            var tontines = await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            ViewBag.StatusFilter = filter;
            return View(tontines);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tontine tontine)
        {
            if (!ModelState.IsValid) return View(tontine);
            tontine.CreatedAt = DateTime.Now;
            tontine.Status = TontineStatus.Draft;
            tontine.OwnerId = _db.CurrentUserId ?? 0;
            _db.Tontines.Add(tontine);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã tạo dây hụi \"{tontine.Name}\"!";
            return RedirectToAction("SetupShares", new { id = tontine.Id });
        }

        /// <summary>Giao diện phân chia phần hụi cho người chơi</summary>
        public async Task<IActionResult> SetupShares(int id)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Shares).ThenInclude(s => s.Player)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tontine == null) return NotFound();

            ViewBag.Players = await _db.Players.OrderBy(p => p.Name).ToListAsync();
            return View(tontine);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetupShares(int id, IFormCollection form)
        {
            try
            {
                Dictionary<int, int> positionMap = new Dictionary<int, int>();
                var tontine = await _db.Tontines.FindAsync(id);
                if (tontine == null) return NotFound();

                for (int i = 1; i <= tontine.TotalShares; i++)
                {
                    string key = $"player_{i}";
                    if (form.ContainsKey(key) && int.TryParse(form[key], out int playerId))
                    {
                        positionMap[i] = playerId;
                    }
                }

                if (positionMap.Count != tontine.TotalShares)
                {
                    TempData["Error"] = "Vui lòng chọn đầy đủ người chơi cho tất cả các phần.";
                    return RedirectToAction("SetupShares", new { id });
                }

                await _drawService.GenerateSharesAsync(id, positionMap);
                TempData["Success"] = "Đã phân chia phần hụi thành công! Dây hụi đang chạy.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("SetupShares", new { id });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveDraft(int id, IFormCollection form)
        {
            try
            {
                var tontine = await _db.Tontines
                    .Include(t => t.Shares)
                    .FirstOrDefaultAsync(t => t.Id == id);
                if (tontine == null) return NotFound();

                if (tontine.Status != TontineStatus.Draft)
                {
                    TempData["Error"] = "Chỉ có thể lưu nháp cho dây hụi chưa bắt đầu.";
                    return RedirectToAction("SetupShares", new { id });
                }

                // Xóa các phần hụi cũ để cập nhật lại từ đầu
                _db.TontineShares.RemoveRange(tontine.Shares);

                for (int i = 1; i <= tontine.TotalShares; i++)
                {
                    string key = $"player_{i}";
                    if (form.ContainsKey(key) && int.TryParse(form[key], out int playerId) && playerId > 0)
                    {
                        _db.TontineShares.Add(new TontineShare
                        {
                            TontineId = id,
                            Position = i,
                            PlayerId = playerId,
                            Status = ShareStatus.Living
                        });
                    }
                }

                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã lưu bản nháp vị trí hụi viên.";
                return RedirectToAction("SetupShares", new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi lưu nháp: " + ex.Message;
                return RedirectToAction("SetupShares", new { id });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Shares).ThenInclude(s => s.Player)
                .Include(t => t.Draws).ThenInclude(d => d.WinningShare).ThenInclude(ws => ws!.Player)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tontine == null) return NotFound();
            
            ViewBag.AllPlayers = await _db.Players.OrderBy(p => p.Name).ToListAsync();
            return View(tontine);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSharePlayer(int shareId, int newPlayerId)
        {
            try
            {
                var share = await _db.TontineShares.FindAsync(shareId);
                if (share == null) return Json(new { success = false, message = "Không tìm thấy phần hụi." });

                share.PlayerId = newPlayerId;
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Đã đổi người chơi cho phần hụi này." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SettleShareEarly(int shareId)
        {
            try
            {
                var share = await _db.TontineShares.FindAsync(shareId);
                if (share == null) return Json(new { success = false, message = "Không tìm thấy phần hụi." });

                if (share.Status != ShareStatus.Dead)
                    return Json(new { success = false, message = "Chỉ có thể tất toán hụi chết." });

                share.IsSettledEarly = true;
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Đã tất toán hụi chết sớm cho phần hụi này." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tontine = await _db.Tontines.FindAsync(id);
            if (tontine == null) return NotFound();
            return View(tontine);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tontine tontine)
        {
            if (id != tontine.Id) return NotFound();
            if (!ModelState.IsValid) return View(tontine);
            _db.Update(tontine);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật dây hụi thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLock(int id)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Shares)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tontine == null) return NotFound();

            if (tontine.Status == TontineStatus.Disabled)
            {
                // Mở khóa: Nếu đã có phần hụi thì quay lại Running, chưa có thì Draft
                tontine.Status = tontine.Shares.Any() ? TontineStatus.Running : TontineStatus.Draft;
                TempData["Success"] = $"Đã mở khóa dây hụi \"{tontine.Name}\"!";
            }
            else if (tontine.Status == TontineStatus.Running || tontine.Status == TontineStatus.Draft)
            {
                tontine.Status = TontineStatus.Disabled;
                TempData["Success"] = $"Đã khóa dây hụi \"{tontine.Name}\"!";
            }
            else
            {
                TempData["Error"] = "Không thể thay đổi trạng thái của dây hụi đã hoàn tất.";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
