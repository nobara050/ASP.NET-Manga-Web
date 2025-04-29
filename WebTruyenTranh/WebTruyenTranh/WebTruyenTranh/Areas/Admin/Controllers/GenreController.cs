using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Models;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Authorize]
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
            var genres = _context.Genre.ToList();
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
                _context.Genre.Add(genre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // Chỉnh sửa
        public IActionResult Edit(int id)
        {
            var genre = _context.Genre.Find(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                _context.Genre.Update(genre);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // Xóa 
        public IActionResult Delete(int id)
        {
            var genre = _context.Genre.Find(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var genre = _context.Genre.Find(id);
            if (genre == null)
                return NotFound();

            _context.Genre.Remove(genre);
            _context.SaveChanges();

            return Ok();
        }

    }
}
