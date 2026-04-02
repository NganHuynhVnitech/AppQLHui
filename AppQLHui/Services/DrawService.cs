using System;
using System.Linq;
using System.Threading.Tasks;
using AppQLHui.Data;
using AppQLHui.DTOs;
using AppQLHui.Models;
using Microsoft.EntityFrameworkCore;

namespace AppQLHui.Services
{
    public class DrawService
    {
        private readonly AppDbContext _db;
        private readonly TontineService _tontineService;

        public DrawService(AppDbContext db, TontineService tontineService)
        {
            _db = db;
            _tontineService = tontineService;
        }

        /// <summary>
        /// Chốt một kỳ khui hụi: tính toán, lưu Draw + Transactions, cập nhật trạng thái phần.
        /// </summary>
        public async Task<Draw> ExecuteDrawAsync(int tontineId, int winningShareId, decimal bidAmount, DateTime drawDate)
        {
            var preview = await _tontineService.CalculateDrawPreviewAsync(tontineId, winningShareId, bidAmount);

            var tontine = await _db.Tontines
                .Include(t => t.Shares)
                .FirstOrDefaultAsync(t => t.Id == tontineId)
                ?? throw new InvalidOperationException("Không tìm thấy dây hụi.");

            // Tạo bản ghi Draw
            var draw = new Draw
            {
                TontineId = tontineId,
                SequenceNumber = preview.SequenceNumber,
                DrawDate = drawDate,
                WinningShareId = winningShareId,
                BidAmount = bidAmount,
                CollectedFee = tontine.CommissionFee
            };
            _db.Draws.Add(draw);
            await _db.SaveChangesAsync(); // lấy draw.Id

            // Tạo Transaction cho từng người chơi
            foreach (var pt in preview.PlayerTransactions)
            {
                var tx = new Transaction
                {
                    DrawId = draw.Id,
                    PlayerId = pt.PlayerId,
                    AmountPay = pt.AmountPay,
                    AmountReceive = pt.AmountReceive,
                    NetTotal = pt.NetTotal,
                    IsSettled = false
                };
                _db.Transactions.Add(tx);
            }

            // Cập nhật phần trúng → Dead + gán WonDrawId
            var winningShare = tontine.Shares.First(s => s.Id == winningShareId);
            winningShare.Status = ShareStatus.Dead;
            winningShare.WonDrawId = draw.Id;

            // Kiểm tra tổng số phần chết – nếu tất cả đã chết thì dây hoàn tất
            var totalDead = tontine.Shares.Count(s => s.Status == ShareStatus.Dead);
            if (totalDead >= tontine.TotalShares)
                tontine.Status = TontineStatus.Completed;
            else if (tontine.Status == TontineStatus.Draft)
                tontine.Status = TontineStatus.Running;

            await _db.SaveChangesAsync();
            return draw;
        }

        /// <summary>Khởi tạo N phần hụi cho một dây hụi vừa được tạo</summary>
        public async Task GenerateSharesAsync(int tontineId, int[] playerIds)
        {
            var tontine = await _db.Tontines.FindAsync(tontineId)
                ?? throw new InvalidOperationException("Không tìm thấy dây hụi.");

            if (playerIds.Length != tontine.TotalShares)
                throw new InvalidOperationException($"Số người chơi ({playerIds.Length}) phải đúng bằng tổng số phần ({tontine.TotalShares}).");

            foreach (var pid in playerIds)
            {
                _db.TontineShares.Add(new TontineShare
                {
                    TontineId = tontineId,
                    PlayerId = pid,
                    Status = ShareStatus.Living
                });
            }

            tontine.Status = TontineStatus.Running;
            await _db.SaveChangesAsync();
        }
    }
}
