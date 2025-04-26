using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models
{
    public class AuthorModel
    {
        [Key] // Khóa chính
        public int AuthorId { get; set; }

        [Required] // Không được để trống
        [StringLength(150)] // Giới hạn độ dài tối đa 150 ký tự
        public string Name { get; set; }
    }
}
