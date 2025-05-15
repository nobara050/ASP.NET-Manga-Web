using System;
using System.ComponentModel.DataAnnotations;
using WebTruyenTranh.Data;

namespace WebTruyenTranh.Models
{
    public class AuthorModel
    {
        [Key] // Khóa chính
        public int AuthorId { get; set; }

        [Required] // Không được để trống
        [StringLength(150)] // Giới hạn độ dài tối đa 150 ký tự
        public string Name { get; set; }
        // =================================================================================
        // =========================    HIỂN THỊ TÁC GIẢ    ================================
        // =================================================================================
        public static List<AuthorModel> GetAll(ApplicationDbContext context)
        {
                return context.Author.ToList();   
        }

        public static AuthorModel? GetById(ApplicationDbContext context, int id)
        {
            return context.Author.Find(id);
        }

        // =================================================================================
        // ========================       THÊM TÁC GIẢ      ================================
        // =================================================================================
        public bool Insert(ApplicationDbContext context)
        {
            try
            {
                context.Author.Add(this);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // =================================================================================
        // =============================    SỬA TÁC GIẢ     ================================
        // =================================================================================
        public bool Update(ApplicationDbContext context)
        {
            try
            {
                context.Author.Update(this);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // =================================================================================
        // =============================    XÓA TÁC GIẢ      ===============================
        // =================================================================================
        public static bool Delete(ApplicationDbContext context, int id)
        {
            var author = context.Author.Find(id);
            if (author == null) return false;

            try
            {
                var bridgeEntries = context.Bridge_Manga_Author.Where(am => am.AuthorId == id).ToList();
                context.Bridge_Manga_Author.RemoveRange(bridgeEntries);

                context.Author.Remove(author);
                context.SaveChanges();
                return true;
            }
            catch
            {
                // Ghi log nếu cần
                return false;
            }
        }
    }
}
