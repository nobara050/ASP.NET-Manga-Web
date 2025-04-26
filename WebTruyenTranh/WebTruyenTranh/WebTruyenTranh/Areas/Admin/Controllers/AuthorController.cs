using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Models;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách
        public IActionResult Index()
        {
            var authors = _context.Authors.ToList();
            return View(authors);
        }

        // Thêm
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AuthorModel author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Add(author);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // Chỉnh sửa
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(AuthorModel author)
        {
            if (ModelState.IsValid)
            {
                _context.Authors.Update(author);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // Xóa
        public IActionResult Delete(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var author = _context.Authors.Find(id);
            if (author == null)
                return NotFound();

            _context.Authors.Remove(author);
            _context.SaveChanges();

            return Ok(); // AJAX sẽ nhận kết quả này
        }

    }
}
