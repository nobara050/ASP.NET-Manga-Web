using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Models;
using WebTruyenTranh.Data;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách
        public IActionResult Index()
        {
            var genres = _context.Genres.ToList();
            return View(genres);
        }

        // Thêm
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                _context.Genres.Add(genre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // Chỉnh sửa
        public IActionResult Edit(int id)
        {
            var genre = _context.Genres.Find(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                _context.Genres.Update(genre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // Xóa 
        public IActionResult Delete(int id)
        {
            var genre = _context.Genres.Find(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var genre = _context.Genres.Find(id);
            if (genre == null)
                return NotFound();

            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return Ok();
        }

    }
}
