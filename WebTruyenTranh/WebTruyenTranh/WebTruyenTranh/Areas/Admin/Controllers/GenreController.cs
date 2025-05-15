using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Models;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookieAuth", Roles = "Admin")]
    public class GenreController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================================
        // ===========================    HIỂN THỊ THỂ LOẠI    =============================
        // =================================================================================
        public IActionResult Index()
        {
            var genres = GenreModel.GetAll(_context);
            return View(genres);
        }

        // =================================================================================
        // ===========================      THÊM THỂ LOẠI      =============================
        // =================================================================================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                if (genre.Insert(_context))
                    return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // =================================================================================
        // ===========================      SỬA THỂ LOẠI       =============================
        // =================================================================================
        public IActionResult Edit(int id)
        {
            var genre = GenreModel.GetById(_context, id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                if (genre.Update(_context))
                    return RedirectToAction(nameof(Index));
            }
            return View(genre);
        }

        // =================================================================================
        // ===========================      XÓA THỂ LOẠI       =============================
        // =================================================================================
        public IActionResult Delete(int id)
        {
            var genre = GenreModel.GetById(_context, id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = GenreModel.Delete(_context, id);
            if (!result) return NotFound();

            return Ok();
        }

    }
}
