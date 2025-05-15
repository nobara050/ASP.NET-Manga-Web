using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Models;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookieAuth", Roles = "Admin")]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================================
        // ============================    HIỂN THỊ TÁC GIẢ    =============================
        // =================================================================================
        public IActionResult Index()
        {
            var authors = AuthorModel.GetAll(_context);
            return View(authors);
        }

        // =================================================================================
        // =============================    THÊM TÁC GIẢ       =============================
        // =================================================================================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AuthorModel author)
        {
            if (ModelState.IsValid)
            {
                if (author.Insert(_context))
                    return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // =================================================================================
        // =============================    SỬA TÁC GIẢ        =============================
        // =================================================================================
        public IActionResult Edit(int id)
        {
            var author = AuthorModel.GetById(_context, id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(AuthorModel author)
        {
            if (ModelState.IsValid)
            {
                if (author.Update(_context))
                    return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // =================================================================================
        // =============================    XÓA TÁC GIẢ        =============================
        // =================================================================================
        public IActionResult Delete(int id)
        {
            var author = AuthorModel.GetById(_context, id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = AuthorModel.Delete(_context, id);
            if (!result) return NotFound();

            return Ok(); // AJAX sẽ nhận kết quả này
        }


    }
}
