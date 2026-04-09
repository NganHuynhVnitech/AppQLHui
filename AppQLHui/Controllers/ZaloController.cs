using AppQLHui.Data;
using AppQLHui.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    [Authorize]
    public class ZaloController : Controller
    {
        private readonly AppDbContext _db;

        public ZaloController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Global Filter already handles OwnerId isolation
            var settings = await _db.ZaloSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new ZaloSettings
                {
                    OwnerId = _db.CurrentUserId ?? 0
                };
                _db.ZaloSettings.Add(settings);
                await _db.SaveChangesAsync();
            }
            return View(settings);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ZaloSettings settings)
        {
            if (!ModelState.IsValid) return View("Index", settings);

            var existing = await _db.ZaloSettings.FirstOrDefaultAsync(s => s.Id == settings.Id);
            if (existing == null) return NotFound();

            // Ownership check (extra safety)
            if (existing.OwnerId != _db.CurrentUserId && !_db.IsAdmin) return Forbid();

            existing.SendDelayMs = settings.SendDelayMs;
            existing.PasteX = settings.PasteX;
            existing.PasteY = settings.PasteY;
            existing.PasteShortcut = settings.PasteShortcut;
            existing.SendShortcut = settings.SendShortcut;
            existing.IsSingleSendOnly = settings.IsSingleSendOnly;
            existing.AutoCopyBill = settings.AutoCopyBill;
            existing.LastUpdated = DateTime.Now;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cấu hình Zalo đã được cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
