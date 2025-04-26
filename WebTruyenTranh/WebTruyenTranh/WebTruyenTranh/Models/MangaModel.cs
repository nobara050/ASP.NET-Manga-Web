using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models
{
    public class MangaModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MangaId { get; set; }  // Khóa chính

        [Required]
        [StringLength(255)]
        public string Title { get; set; }  // Tiêu đề truyện

        [StringLength(1000)]
        public string? Description { get; set; }  // Mô tả truyện

        [StringLength(1000)]
        public string? CoverImage { get; set; } = "/uploads/nocover.png"; // Ảnh bìa

        [StringLength(1000)]
        public string? BackgroundImage { get; set; }  // Ảnh nền

        [StringLength(50)]
        public string? Status { get; set; }  // Trạng thái (Đang ra, Hoàn thành, Tạm ngưng)

        public int? UserId { get; set; }  // Người tạo

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Ngày tạo, mặc định là ngày hiện tại

        [NotMapped]
        public IFormFile? CoverImageFile { get; set; }  // Tệp ảnh bìa tải lên

        [NotMapped]
        public IFormFile? BackgroundImageFile { get; set; }  // Tệp ảnh nền tải lên
    }
}
