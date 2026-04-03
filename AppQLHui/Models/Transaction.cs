using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppQLHui.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public int DrawId { get; set; }
        public Draw Draw { get; set; } = null!;

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Số tiền phải đóng")]
        public decimal AmountPay { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        [Display(Name = "Số tiền được nhận")]
        public decimal AmountReceive { get; set; }

        /// <summary>Số tiền thực nhận/đóng sau khi bù trừ. Dương = nhận, Âm = phải đóng</summary>
        [Column("NetAmount", TypeName = "decimal(18,0)")]
        [Display(Name = "Thực nhận/đóng (sau bù trừ)")]
        public decimal NetTotal { get; set; }

        [Display(Name = "Đã thanh toán")]
        public bool IsSettled { get; set; } = false;
    }
}
