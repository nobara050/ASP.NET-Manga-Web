using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models
{
    public class GenreModel
    {
        [Key] // Đánh dấu GenreId là khóa chính
        public int GenreId { get; set; }

        [Required] // Không cho phép để trống
        [StringLength(100)] // Giới hạn độ dài
        public string Name { get; set; }
    }
}
