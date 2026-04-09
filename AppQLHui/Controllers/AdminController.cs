using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppQLHui.Data;
using AppQLHui.Models;

namespace AppQLHui.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Users()
        {
            // Admin sees all users. We use IgnoreQueryFilters for safety though Users doesn't have one usually.
            var users = await _db.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
            return View(users);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(string username, string fullName, string password, UserRole role, 
            string? phone, string? bankName, string? bankAccountNumber, string? bankAccountName)
        {
            if (await _db.Users.IgnoreQueryFilters().AnyAsync(u => u.Username == username))
            {
                TempData["Error"] = "Tên đăng nhập đã tồn tại!";
                return RedirectToAction(nameof(Users));
            }

            var user = new AppUser
            {
                Username = username,
                FullName = fullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                Phone = phone,
                BankName = bankName,
                BankAccountNumber = bankAccountNumber,
                BankAccountName = bankAccountName,
                CreatedAt = DateTime.Now
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã tạo tài khoản {role} cho {fullName}!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null || user.Username == "admin")
            {
                TempData["Error"] = "Không thể thay đổi trạng thái tài khoản này!";
                return RedirectToAction(nameof(Users));
            }

            user.IsEnabled = !user.IsEnabled;
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã {(user.IsEnabled ? "mở" : "khóa")} tài khoản {user.FullName}!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null || user.Username == "admin") 
            {
                TempData["Error"] = "Không thể xóa tài khoản này!";
                return RedirectToAction(nameof(Users));
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đã xóa tài khoản người dùng!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, string fullName, UserRole role,
            string? phone, string? bankName, string? bankAccountNumber, string? bankAccountName)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null || user.Username == "admin")
            {
                TempData["Error"] = "Không tìm thấy người dùng hoặc không thể sửa tài khoản này!";
                return RedirectToAction(nameof(Users));
            }

            user.FullName = fullName;
            user.Role = role;
            user.Phone = phone;
            user.BankName = bankName;
            user.BankAccountNumber = bankAccountNumber;
            user.BankAccountName = bankAccountName;
            
            await _db.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null || user.Username == "admin")
            {
                TempData["Error"] = "Không thể reset mật khẩu cho tài khoản này!";
                return RedirectToAction(nameof(Users));
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456");
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đã reset mật khẩu của {user.FullName} về mặc định: 123456";
            return RedirectToAction(nameof(Users));
        }
    }
}
