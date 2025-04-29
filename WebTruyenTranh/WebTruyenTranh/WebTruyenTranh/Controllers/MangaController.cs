using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace WebTruyenTranh.Controllers
{
    public class MangaController : Controller
    {
        public IActionResult ReadManga()
        {
            // Kiểm tra thư mục tồn tại
            string imgDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            if (!Directory.Exists(imgDir))
            {
                // Log lỗi nếu thư mục không tồn tại
                Console.WriteLine($"Thư mục {imgDir} không tồn tại");
            }

            // Danh sách đường dẫn ảnh cục bộ trong wwwroot/images
            var imageUrls = new List<string>
            {
                "/images/manga1.jpg",
                "/images/manga2.jpg",
                "/images/manga3.jpg"
            };

            // Kiểm tra xem file có tồn tại không
            foreach (var url in imageUrls)
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", url.TrimStart('/'));
                if (!System.IO.File.Exists(fullPath))
                {
                    Console.WriteLine($"File {fullPath} không tồn tại");
                }
            }

            return View(imageUrls);
        }
    }
}