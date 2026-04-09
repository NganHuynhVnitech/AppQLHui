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
            decimal bidAmount,
            decimal otherDeduction = 0,
            string? otherDeductionNote = null)
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
                .Select(d => (int?)d.SequenceNumber)
                .MaxAsync()) ?? 0;
            nextSeq += 1;

            // Phân loại các phần trong dây (không tính phần vừa trúng khi tính tiền đóng)
            // Strict Professional Logic: Total paying shares = TotalShares - 1 (the winner)
            int deadCount = nextSeq - 1;
            int livingCount = tontine.TotalShares - nextSeq; 

            // Phân loại các phần trong dây để tính breakdown từng người
            var allShares = tontine.Shares.OrderBy(s => s.Position).Take(tontine.TotalShares).ToList();
            var deadShares = allShares.Where(s => s.Status == ShareStatus.Dead).ToList();
            var livingSharesExcludingWinner = allShares
                .Where(s => s.Status == ShareStatus.Living && s.Id != winningShareId)
                .ToList();

            decimal totalFromDead = deadCount * tontine.BaseAmount;
            decimal totalFromLiving = livingCount * (tontine.BaseAmount - bidAmount);
            
            // Professional Logic: Fee collected based on FeeType
            // Before: only seq 1 pays. After: everyone pays.
            decimal curCollectedFee = 0;
            if (tontine.FeeType == FeeType.After)
                curCollectedFee = tontine.CommissionFee;
            else if (tontine.FeeType == FeeType.Before && nextSeq == 1)
                curCollectedFee = tontine.CommissionFee;

            decimal totalReceived = totalFromDead + totalFromLiving - curCollectedFee;

            // Fetch Old Debts to deduct from Winner
            decimal winnerOldDebt = await _db.Transactions
                .Where(t => t.PlayerId == winningShare.PlayerId && !t.IsSettled && t.NetTotal < 0)
                .SumAsync(t => Math.Abs(t.NetTotal) - t.PaidAmount);

            // Build per-player aggregation
            var playerMap = new Dictionary<int, (string Name, string? Phone, int LivingCount, int DeadCount, decimal Pay, decimal Receive, List<int> Positions)>();

            void EnsurePlayer(TontineShare s)
            {
                if (!playerMap.ContainsKey(s.PlayerId))
                    playerMap[s.PlayerId] = (s.Player.Name, s.Player.Phone, 0, 0, 0m, 0m, new List<int>());
                
                if (!playerMap[s.PlayerId].Positions.Contains(s.Position))
                    playerMap[s.PlayerId].Positions.Add(s.Position);
            }

            foreach (var s in deadShares)
            {
                EnsurePlayer(s);
                var cur = playerMap[s.PlayerId];
                decimal amtToPay = s.IsSettledEarly ? 0m : tontine.BaseAmount;
                playerMap[s.PlayerId] = (cur.Name, cur.Phone, cur.LivingCount, cur.DeadCount + 1, cur.Pay + amtToPay, cur.Receive, cur.Positions);
            }

            foreach (var s in livingSharesExcludingWinner)
            {
                EnsurePlayer(s);
                var cur = playerMap[s.PlayerId];
                playerMap[s.PlayerId] = (cur.Name, cur.Phone, cur.LivingCount + 1, cur.DeadCount, cur.Pay + (tontine.BaseAmount - bidAmount), cur.Receive, cur.Positions);
            }

            EnsurePlayer(winningShare);
            var winner = playerMap[winningShare.PlayerId];
            playerMap[winningShare.PlayerId] = (winner.Name, winner.Phone, winner.LivingCount, winner.DeadCount, winner.Pay, winner.Receive + totalReceived, winner.Positions);

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
                OldDebtDeduction = kv.Key == winningShare.PlayerId ? winnerOldDebt : 0,
                OtherDeduction = kv.Key == winningShare.PlayerId ? otherDeduction : 0,
                OtherDeductionNote = kv.Key == winningShare.PlayerId ? otherDeductionNote : null,
                IsWinner = kv.Key == winningShare.PlayerId
            }).OrderByDescending(x => x.IsWinner).ThenBy(x => x.PlayerName).ToList();

            return new DrawPreviewDto
            {
                TontineId = tontineId,
                TontineName = tontine.Name,
                SequenceNumber = nextSeq,
                BaseAmount = tontine.BaseAmount,
                BidAmount = bidAmount,
                CommissionFee = curCollectedFee,
                WinningShareId = winningShareId,
                WinnerPlayerId = winningShare.PlayerId,
                WinnerPlayerName = winningShare.Player.Name,
                CountLivingPortions = livingCount,
                CountDeadPortions = deadCount,
                TotalFromLiving = totalFromLiving,
                TotalFromDead = totalFromDead,
                TotalReceived = totalReceived,
                OldDebtDeduction = winnerOldDebt,
                OtherDeduction = otherDeduction,
                OtherDeductionNote = otherDeductionNote,
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
