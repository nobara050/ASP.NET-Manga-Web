using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models
{
    public class CommentReportModel
    {
        [Key]
        public int ReportId { get; set; }
        [Required]
        public int CommentId { get; set; }
        [Required]
        public int AccountId { get; set; }
        public string Reason { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
