using System.ComponentModel.DataAnnotations;

namespace AppQLHui.Models
{
    public enum ShareStatus { Living, Dead }

    public class TontineShare
    {
        public int Id { get; set; }

        public int TontineId { get; set; }
        public Tontine Tontine { get; set; } = null!;

        [Display(Name = "Số thứ tự")]
        public int Position { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;

        [Display(Name = "Trạng thái phần")]
        public ShareStatus Status { get; set; } = ShareStatus.Living;

        /// <summary>Kỳ draw mà phần này đã hốt (null nếu chưa hốt)</summary>
        public int? WonDrawId { get; set; }
        public Draw? WonDraw { get; set; }
    }
}
