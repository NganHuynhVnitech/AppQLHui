using System.ComponentModel.DataAnnotations;

namespace AppQLHui.Models
{
    public class ZaloSettings
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Độ trễ khi gửi (ms)")]
        public int SendDelayMs { get; set; } = 1000;

        [Display(Name = "Tọa độ dán (X)")]
        public int PasteX { get; set; } = 0;

        [Display(Name = "Tọa độ dán (Y)")]
        public int PasteY { get; set; } = 0;

        [Display(Name = "Phím tắt dán")]
        [StringLength(50)]
        public string PasteShortcut { get; set; } = "Ctrl+V";

        [Display(Name = "Phím tắt gửi")]
        [StringLength(50)]
        public string SendShortcut { get; set; } = "Enter";

        [Display(Name = "Gửi 1N (Chỉ 1 người)")]
        public bool IsSingleSendOnly { get; set; } = true;

        [Display(Name = "Tự động copy Bill")]
        public bool AutoCopyBill { get; set; } = true;

        public int OwnerId { get; set; }
        public AppUser Owner { get; set; } = null!;

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
