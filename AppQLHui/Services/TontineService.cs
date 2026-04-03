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
    public class TontineService
    {
        private readonly AppDbContext _db;

        public TontineService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Tính toán preview một kỳ khui hụi. Trả về DrawPreviewDto với danh sách
        /// PlayerTransactionDto đã được net-balance theo từng PlayerId.
        /// </summary>
        public async Task<DrawPreviewDto> CalculateDrawPreviewAsync(
            int tontineId,
            int winningShareId,
            decimal bidAmount)
        {
            var tontine = await _db.Tontines
                .Include(t => t.Shares)
                    .ThenInclude(s => s.Player)
                .FirstOrDefaultAsync(t => t.Id == tontineId)
                ?? throw new InvalidOperationException("Không tìm thấy dây hụi.");

            var winningShare = tontine.Shares.FirstOrDefault(s => s.Id == winningShareId)
                ?? throw new InvalidOperationException("Không tìm thấy phần hụi trúng.");

            if (winningShare.Status != ShareStatus.Living)
                throw new InvalidOperationException("Phần hụi đã hốt trước đó rồi.");

            int nextSeq = (await _db.Draws
                .Where(d => d.TontineId == tontineId)
                .CountAsync()) + 1;

            // Phân loại các phần trong dây (không tính phần vừa trúng khi tính tiền đóng)
            var allShares = tontine.Shares.ToList();
            var deadShares = allShares.Where(s => s.Status == ShareStatus.Dead).ToList();
            var livingSharesExcludingWinner = allShares
                .Where(s => s.Status == ShareStatus.Living && s.Id != winningShareId)
                .ToList();

            decimal totalFromDead = deadShares.Count * tontine.BaseAmount;
            decimal totalFromLiving = livingSharesExcludingWinner.Count * (tontine.BaseAmount - bidAmount);
            
            // Professional Logic: Total Received = (Living - 1) * (Base - Bid) + (Dead * Base) - Thao
            // (Note: Winner's current winning share is not part of totalFromLiving or totalFromDead here based on above logic)
            decimal totalReceived = totalFromDead + totalFromLiving - tontine.CommissionFee;

            // Build per-player aggregation
            // Key: PlayerId → (Name, Phone, LivingCount, DeadCount, Pay, Receive, Positions)
            var playerMap = new Dictionary<int, (string Name, string? Phone, int LivingCount, int DeadCount, decimal Pay, decimal Receive, List<int> Positions)>();

            void EnsurePlayer(TontineShare s)
            {
                if (!playerMap.ContainsKey(s.PlayerId))
                    playerMap[s.PlayerId] = (s.Player.Name, s.Player.Phone, 0, 0, 0m, 0m, new List<int>());
                
                if (!playerMap[s.PlayerId].Positions.Contains(s.Position))
                    playerMap[s.PlayerId].Positions.Add(s.Position);
            }

            // Dead shares → pay BaseAmount each
            foreach (var s in deadShares)
            {
                EnsurePlayer(s);
                var cur = playerMap[s.PlayerId];
                playerMap[s.PlayerId] = (cur.Name, cur.Phone, cur.LivingCount, cur.DeadCount + 1, cur.Pay + tontine.BaseAmount, cur.Receive, cur.Positions);
            }

            // Living shares (excl. winner) → pay (M - B) each
            foreach (var s in livingSharesExcludingWinner)
            {
                EnsurePlayer(s);
                var cur = playerMap[s.PlayerId];
                playerMap[s.PlayerId] = (cur.Name, cur.Phone, cur.LivingCount + 1, cur.DeadCount, cur.Pay + (tontine.BaseAmount - bidAmount), cur.Receive, cur.Positions);
            }

            // Winner receives totalReceived, but still pays for their OTHER shares
            // (winner's own winning share doesn't pay in THIS draw)
            EnsurePlayer(winningShare);
            var winner = playerMap[winningShare.PlayerId];
            playerMap[winningShare.PlayerId] = (winner.Name, winner.Phone, winner.LivingCount, winner.DeadCount, winner.Pay, winner.Receive + totalReceived, winner.Positions);

            // Build result list with NetTotal = Receive - Pay
            var result = playerMap.Select(kv => new PlayerTransactionDto
            {
                PlayerId = kv.Key,
                PlayerName = kv.Value.Name,
                Phone = kv.Value.Phone,
                LivingShareCount = kv.Value.LivingCount,
                DeadShareCount = kv.Value.DeadCount,
                AmountPay = kv.Value.Pay,
                AmountReceive = kv.Value.Receive,
                NetTotal = kv.Value.Receive - kv.Value.Pay,
                IsWinner = kv.Key == winningShare.PlayerId
            }).OrderByDescending(x => x.IsWinner).ThenBy(x => x.PlayerName).ToList();

            return new DrawPreviewDto
            {
                TontineId = tontineId,
                TontineName = tontine.Name,
                SequenceNumber = nextSeq,
                BaseAmount = tontine.BaseAmount,
                BidAmount = bidAmount,
                CommissionFee = tontine.CommissionFee,
                WinningShareId = winningShareId,
                WinnerPlayerId = winningShare.PlayerId,
                WinnerPlayerName = winningShare.Player.Name,
                TotalFromLiving = totalFromLiving,
                TotalFromDead = totalFromDead,
                TotalReceived = totalReceived,
                PlayerTransactions = result
            };
        }

        /// <summary>Lấy danh sách phần sống của dây hụi (dùng cho panel chọn người hốt)</summary>
        public async Task<List<TontineShare>> GetLivingSharesAsync(int tontineId)
        {
            return await _db.TontineShares
                .Include(ts => ts.Player)
                .Where(ts => ts.TontineId == tontineId && ts.Status == ShareStatus.Living)
                .ToListAsync();
        }
    }
}
