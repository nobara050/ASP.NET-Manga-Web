using System.ComponentModel.DataAnnotations;
namespace WebTruyenTranh.Models
{
    public class ContentModel
    {
        [Key]
        public int ContentId { get; set; }
        [Required]
        public int MangaId { get; set; }
        [Required]
        public int ChapterId { get; set; }
        [Required]
        public int ContentNum { get; set; }
        [StringLength(1000)]
        public string Image { get; set; }
    }
}
