using System.ComponentModel.DataAnnotations;
namespace WebTruyenTranh.Models
{
    public class ChapterModel
    {
        [Key] 
        public int ChapterId { get; set; }
        [Required]
        public int MangaId { get; set; }

        [Required] 
        [StringLength(255)] 
        public string Title { get; set; }

        [StringLength(255)]  
        public string? Alias { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
    }
}
