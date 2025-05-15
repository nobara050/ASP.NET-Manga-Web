using System.ComponentModel.DataAnnotations;
using WebTruyenTranh.Data;

namespace WebTruyenTranh.Models
{
    public class GenreModel
    {
        [Key] // Đánh dấu GenreId là khóa chính
        public int GenreId { get; set; }

        [Required] // Không cho phép để trống
        [StringLength(100)] // Giới hạn độ dài
        public string Name { get; set; }

        // =================================================================================
        // ===========================    HIỂN THỊ THỂ LOẠI    =============================
        // =================================================================================
        public static List<GenreModel> GetAll(ApplicationDbContext context)
        {
            return context.Genre.ToList();
        }

        public static GenreModel? GetById(ApplicationDbContext context, int id)
        {
            return context.Genre.Find(id);
        }

        // =================================================================================
        // ===========================      THÊM THỂ LOẠI      =============================
        // =================================================================================
        public bool Insert(ApplicationDbContext context)
        {
            try
            {
                context.Genre.Add(this);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // =================================================================================
        // ===========================       SỬA THỂ LOẠI      =============================
        // =================================================================================
        public bool Update(ApplicationDbContext context)
        {
            try
            {
                context.Genre.Update(this);
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // =================================================================================
        // ===========================       XÓA THỂ LOẠI      =============================
        // =================================================================================
        public static bool Delete(ApplicationDbContext context, int id)
        {
            var genre = context.Genre.Find(id);
            if (genre == null) return false;

            try
            {
                var bridgeEntries = context.Bridge_Manga_Genre.Where(am => am.GenreId == id).ToList();
                context.Bridge_Manga_Genre.RemoveRange(bridgeEntries);

                context.Genre.Remove(genre);
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
