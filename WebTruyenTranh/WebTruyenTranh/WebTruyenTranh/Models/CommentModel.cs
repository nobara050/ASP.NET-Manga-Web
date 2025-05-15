using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebTruyenTranh.Data;

namespace WebTruyenTranh.Models
{
    public class CommentModel
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public int MangaId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        [Required]
        public int AccountId { get; set; }
        public int? ParentCommentId { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public bool IsDeleted { get; set; }

        // ======================================
        [NotMapped]
        public AccountModel? User { get; set; }
        public AccountModel? LoadUser(ApplicationDbContext context)
        {
            User = context.Account.FirstOrDefault(a => a.AccountId == AccountId);
            return User;
        }
        [NotMapped]
        public int LikeCount { get; set; } = 0;

        public int GetLikeCount(ApplicationDbContext context)
        {
            this.LikeCount = context.CommentLike.Count(l => l.CommentId == this.CommentId);
            return this.LikeCount;
        }
        [NotMapped]
        public List<CommentModel> Replies { get; set; } = new();
        public List<CommentModel> SortReplies()
        {
            if (Replies == null || Replies.Count == 0) return new();

            Replies = Replies.OrderBy(r => r.CreatedAt).ToList();
            return Replies;
        }

    }
}
