using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTruyenTranh.Models;
using WebTruyenTranh.Models.ViewModel;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class MangaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public MangaController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }


        // =================================================================================
        // ===========================    HIỂN THỊ DANH SÁCH  ==============================
        // =================================================================================
        public IActionResult Index()
        {
            List<MangaAuthorGenreViewModel> ListMangaView = new List<MangaAuthorGenreViewModel>();
            var mangas = _context.Mangas.ToList();
            var genres = _context.Genres.ToList();
            var authors = _context.Authors.ToList();
            var mangaGenres = _context.Bridge_Manga_Genre.ToList();
            var mangaAuthors = _context.Bridge_Manga_Author.ToList();
            foreach (var manga in mangas)
            {
                var relatedGenreIds = mangaGenres
                    .Where(bg => bg.MangaId == manga.MangaId)
                    .Select(bg => bg.GenreId)
                    .ToList();

                var relatedGenres = genres
                    .Where(g => relatedGenreIds.Contains(g.GenreId))
                    .ToList();

                var relatedAuthorIds = mangaAuthors
                    .Where(ba => ba.MangaId == manga.MangaId)
                    .Select(ba => ba.AuthorId)
                    .ToList();

                var relatedAuthors = authors
                    .Where(a => relatedAuthorIds.Contains(a.AuthorId))
                    .ToList();

                ListMangaView.Add(new MangaAuthorGenreViewModel
                {
                    MangaModel = manga,
                    GenresModel = relatedGenres,
                    AuthorsModel = relatedAuthors
                });
            }

            return View(ListMangaView);
        }


        // =================================================================================
        // ==============================    THÊM TRUYỆN    ================================
        // =================================================================================
        public IActionResult Create()
        {
            var mangaViewModel = new MangaAuthorGenreViewModel
            {
                MangaModel = new MangaModel(),
                GenresModel = new List<GenreModel>(),
                AuthorsModel = new List<AuthorModel>(),
                GenresModelAll = _context.Genres.ToList(),
                AuthorsModelAll = _context.Authors.ToList()
            };
            return View(mangaViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MangaAuthorGenreViewModel mangaViewModel)
        {
            if (ModelState.IsValid)
            {
                // Xử lý ảnh bìa
                if (mangaViewModel.MangaModel.CoverImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(mangaViewModel.MangaModel.CoverImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await mangaViewModel.MangaModel.CoverImageFile.CopyToAsync(fileStream);
                    }
                    mangaViewModel.MangaModel.CoverImage = "/uploads/" + uniqueFileName;
                }

                // Xử lý ảnh nền
                if (mangaViewModel.MangaModel.BackgroundImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(mangaViewModel.MangaModel.BackgroundImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await mangaViewModel.MangaModel.BackgroundImageFile.CopyToAsync(fileStream);
                    }
                    mangaViewModel.MangaModel.BackgroundImage = "/uploads/" + uniqueFileName;
                }

                // Đặt UserId luôn bằng 0 (admin khi thêm bằng tài khoản admin)
                mangaViewModel.MangaModel.UserId = 0;

                _context.Mangas.Add(mangaViewModel.MangaModel);
                await _context.SaveChangesAsync();

                // Thêm thể loại bảng Bridge_Manga_Genre
                if (mangaViewModel.SelectedGenreIds != null && mangaViewModel.SelectedGenreIds.Any())
                {
                    foreach (var genreId in mangaViewModel.SelectedGenreIds)
                    {
                        _context.Bridge_Manga_Genre.Add(new Bridge_Manga_GenreModel
                        {
                            MangaId = mangaViewModel.MangaModel.MangaId,
                            GenreId = genreId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                // Thêm thể loại bảng Bridge_Manga_Author
                if (mangaViewModel.ListAuthorName != null)
                {
                    foreach (var name in mangaViewModel.ListAuthorName)
                    {
                        var existingAuthor = await _context.Authors
                            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

                        int authorId;

                        if (existingAuthor != null)
                        {
                            authorId = existingAuthor.AuthorId;
                        }
                        else
                        {
                            var newAuthor = new AuthorModel { Name = name };
                            _context.Authors.Add(newAuthor);
                            await _context.SaveChangesAsync(); // cần lưu lại để lấy được AuthorId
                            authorId = newAuthor.AuthorId;
                        }

                        _context.Bridge_Manga_Author.Add(new Bridge_Manga_AuthorModel
                        {
                            MangaId = mangaViewModel.MangaModel.MangaId,
                            AuthorId = authorId
                        });
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            
            mangaViewModel.GenresModelAll = _context.Genres.ToList();
            mangaViewModel.AuthorsModelAll = _context.Authors.ToList();
            return View(mangaViewModel);
        }



        // =================================================================================
        // ================================   CHỈNH SỬA  ===================================
        // =================================================================================
        public async Task<IActionResult> Edit(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null) return NotFound();

            var viewModel = new MangaAuthorGenreViewModel
            {
                MangaModel = manga,
                GenresModelAll = _context.Genres.ToList(),
                AuthorsModelAll = _context.Authors.ToList(),
                SelectedGenreIds = _context.Bridge_Manga_Genre
                                        .Where(bg => bg.MangaId == id)
                                        .Select(bg => bg.GenreId)
                                        .ToList(),
                ListAuthorName = (from ba in _context.Bridge_Manga_Author
                                  join a in _context.Authors on ba.AuthorId equals a.AuthorId
                                  where ba.MangaId == id
                                  select a.Name).ToList()

            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDo(MangaAuthorGenreViewModel viewModel)
        {
            // Tìm Manga trong cơ sở dữ liệu
            var existingManga = await _context.Mangas.FindAsync(viewModel.MangaModel.MangaId);
            if (existingManga == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            existingManga.Title = viewModel.MangaModel.Title;
            existingManga.Description = viewModel.MangaModel.Description;
            existingManga.Status = viewModel.MangaModel.Status;

            // Kiểm tra ảnh bìa mới
            if (viewModel.MangaModel.CoverImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(existingManga.CoverImage))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, existingManga.CoverImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Lưu ảnh bìa mới
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewModel.MangaModel.CoverImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await viewModel.MangaModel.CoverImageFile.CopyToAsync(fileStream);
                }
                existingManga.CoverImage = "/uploads/" + uniqueFileName; // Cập nhật đường dẫn ảnh bìa
            }

            // Kiểm tra ảnh nền mới
            if (viewModel.MangaModel.BackgroundImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // Đảm bảo thư mục tồn tại

                // Xóa ảnh nền cũ nếu có
                if (!string.IsNullOrEmpty(existingManga.BackgroundImage))
                {
                    string oldBackgroundPath = Path.Combine(_webHostEnvironment.WebRootPath, existingManga.BackgroundImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldBackgroundPath))
                    {
                        System.IO.File.Delete(oldBackgroundPath);
                    }
                }

                // Lưu ảnh nền mới
                string uniqueBackgroundName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewModel.MangaModel.BackgroundImageFile.FileName);
                string backgroundPath = Path.Combine(uploadsFolder, uniqueBackgroundName);
                using (var backgroundStream = new FileStream(backgroundPath, FileMode.Create))
                {
                    await viewModel.MangaModel.BackgroundImageFile.CopyToAsync(backgroundStream);
                }
                existingManga.BackgroundImage = "/uploads/" + uniqueBackgroundName; // Cập nhật đường dẫn ảnh nền
            }


            // Cập nhật lại genre
            var existingGenres = _context.Bridge_Manga_Genre.Where(bg => bg.MangaId == viewModel.MangaModel.MangaId);
                _context.Bridge_Manga_Genre.RemoveRange(existingGenres);
                if (viewModel.SelectedGenreIds != null)
                {
                    foreach (var genreId in viewModel.SelectedGenreIds)
                    {
                        _context.Bridge_Manga_Genre.Add(new Bridge_Manga_GenreModel { MangaId = viewModel.MangaModel.MangaId, GenreId = genreId });
                    }
                }

            // Cập nhật lại author
            var existingAuthors = _context.Bridge_Manga_Author.Where(ba => ba.MangaId == viewModel.MangaModel.MangaId);
            _context.Bridge_Manga_Author.RemoveRange(existingAuthors);
            if (viewModel.ListAuthorName != null)
            {
                foreach (var name in viewModel.ListAuthorName)
                {
                    var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
                    if (author == null)
                    {
                        author = new AuthorModel { Name = name };
                        _context.Authors.Add(author);
                        await _context.SaveChangesAsync();
                    }
                    _context.Bridge_Manga_Author.Add(new Bridge_Manga_AuthorModel { MangaId = viewModel.MangaModel.MangaId, AuthorId = author.AuthorId });
                }
            }


            // Lưu các thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Chuyển hướng về trang Index sau khi lưu
        }


        // =================================================================================
        // ==================================   XÓA  =======================================
        // =================================================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manga = await _context.Mangas.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }

            // Xóa ảnh bìa nếu có
            if (!string.IsNullOrEmpty(manga.CoverImage))
            {
                string coverPath = Path.Combine(_webHostEnvironment.WebRootPath, manga.CoverImage.TrimStart('/'));
                if (System.IO.File.Exists(coverPath))
                {
                    System.IO.File.Delete(coverPath);
                }
            }

            // Xóa ảnh nền nếu có
            if (!string.IsNullOrEmpty(manga.BackgroundImage))
            {
                string backgroundPath = Path.Combine(_webHostEnvironment.WebRootPath, manga.BackgroundImage.TrimStart('/'));
                if (System.IO.File.Exists(backgroundPath))
                {
                    System.IO.File.Delete(backgroundPath);
                }
            }

            // Xóa bridge thể loại
            var mangaGenres = _context.Bridge_Manga_Genre.Where(bg => bg.MangaId == id);
            _context.Bridge_Manga_Genre.RemoveRange(mangaGenres);

            // Xóa bridge tác giả
            var mangaAuthors = _context.Bridge_Manga_Author.Where(ba => ba.MangaId == id);
            _context.Bridge_Manga_Author.RemoveRange(mangaAuthors);

            // Xóa manga
            _context.Mangas.Remove(manga);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
