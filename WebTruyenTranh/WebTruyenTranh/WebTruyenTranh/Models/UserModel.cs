using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }
        [Required]
        [StringLength(255)]
        public string Username { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string Password { get; set; }
        [StringLength(1000)]
        public string Avatar { get; set; }
        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
