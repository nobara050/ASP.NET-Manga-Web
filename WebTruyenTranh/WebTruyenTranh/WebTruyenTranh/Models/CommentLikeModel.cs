using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models
{
    public class CommentLikeModel
    {
        [Key]
        public int LikeId { get; set; }
        [Required]
        public int CommentId { get; set; }
        [Required]
        public int AccountId { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
