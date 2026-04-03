using AppQLHui.Data;
using AppQLHui.Models;
using AppQLHui.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    public class TontineController : Controller
    {
        private readonly AppDbContext _db;
        private readonly DrawService _drawService;

        public TontineController(AppDbContext db, DrawService drawService)
        {
            _db = db;
            _drawService = drawService;
        }

        public async Task<IActionResult> Index()
        {
            var tontines = await _db.Tontines
                .Include(t => t.Shares)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
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
            _db.Tontines.Add(tontine);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã tạo dây hụi \"{tontine.Name}\"!";
            return RedirectToAction("SetupShares", new { id = tontine.Id });
        }

        /// <summary>Giao diện phân chia phần hụi cho người chơi</summary>
        public async Task<IActionResult> SetupShares(int id)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Shares)
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

        public async Task<IActionResult> Details(int id)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Shares).ThenInclude(s => s.Player)
                .Include(t => t.Draws).ThenInclude(d => d.WinningShare).ThenInclude(ws => ws!.Player)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tontine == null) return NotFound();
            return View(tontine);
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
    }
}
