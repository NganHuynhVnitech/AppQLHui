using AppQLHui.Data;
using AppQLHui.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Controllers
{
    public class PlayerController : Controller
    {
        private readonly AppDbContext _db;

        public PlayerController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var players = await _db.Players.OrderBy(p => p.Name).ToListAsync();
            return View(players);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Player player)
        {
            if (!ModelState.IsValid) return View(player);
            player.CreatedAt = DateTime.Now;
            _db.Players.Add(player);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã thêm hụi viên \"{player.Name}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Player player)
        {
            if (id != player.Id) return NotFound();
            if (!ModelState.IsValid) return View(player);
            _db.Update(player);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _db.Players.FindAsync(id);
            if (player != null)
            {
                _db.Players.Remove(player);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Đã xoá hụi viên!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
