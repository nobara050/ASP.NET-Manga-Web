using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebTruyenTranh.Models;
using WebTruyenTranh.Models.ViewModel;
using WebTruyenTranh.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace WebTruyenTranh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminCookieAuth", Roles = "Admin")]
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
        // ===========================        HIỂN THỊ         =============================
        // =================================================================================
        public IActionResult Index(int page = 1)
        {
            var model = ListMangaAuthorGenreViewModel.GetPagedData(_context, page);
            return View(model);
        }


        // =================================================================================
        // ==============================    THÊM TRUYỆN    ================================
        // =================================================================================
        public IActionResult Create()
        {
            var mangaViewModel = new MangaAuthorGenreViewModel
            {
                MangaModel = new MangaModel(),
                GenresModels = new List<GenreModel>(),
                AuthorsModels = new List<AuthorModel>(),
                GenresModelAll = _context.Genre.ToList(),
                AuthorsModelAll = _context.Author.ToList()
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
                else
                {
                    // Nếu không upload ảnh bìa thì gán ảnh mặc định
                    mangaViewModel.MangaModel.CoverImage = "/uploads/no_cover.png";
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

                _context.Manga.Add(mangaViewModel.MangaModel);
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
                        var existingAuthor = await _context.Author
                            .FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());

                        int authorId;

                        if (existingAuthor != null)
                        {
                            authorId = existingAuthor.AuthorId;
                        }
                        else
                        {
                            var newAuthor = new AuthorModel { Name = name };
                            _context.Author.Add(newAuthor);
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

            mangaViewModel.GenresModelAll = _context.Genre.ToList();
            mangaViewModel.AuthorsModelAll = _context.Author.ToList();
            return View(mangaViewModel);
        }



        // =================================================================================
        // ================================   CHỈNH SỬA  ===================================
        // =================================================================================
        public async Task<IActionResult> Edit(int id)
        {
            var manga = await _context.Manga.FindAsync(id);
            if (manga == null) return NotFound();

            var viewModel = new MangaAuthorGenreViewModel
            {
                MangaModel = manga,
                GenresModelAll = _context.Genre.ToList(),
                AuthorsModelAll = _context.Author.ToList(),
                SelectedGenreIds = _context.Bridge_Manga_Genre
                                        .Where(bg => bg.MangaId == id)
                                        .Select(bg => bg.GenreId)
                                        .ToList(),
                ListAuthorName = (from ba in _context.Bridge_Manga_Author
                                  join a in _context.Author on ba.AuthorId equals a.AuthorId
                                  where ba.MangaId == id
                                  select a.Name).ToList()

            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MangaAuthorGenreViewModel viewModel)
        {
            // Tìm Manga trong cơ sở dữ liệu
            var existingManga = await _context.Manga.FindAsync(viewModel.MangaModel.MangaId);
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
                    var author = await _context.Author.FirstOrDefaultAsync(a => a.Name.ToLower() == name.ToLower());
                    if (author == null)
                    {
                        author = new AuthorModel { Name = name };
                        _context.Author.Add(author);
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

        public async Task<IActionResult> Delete(int id)
        {
            var manga = await _context.Manga.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }
            return View(manga);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manga = await _context.Manga.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }

            // Xử lý xóa ảnh bìa nếu không phải ảnh mặc định
            if (!string.IsNullOrEmpty(manga.CoverImage))
            {
                string coverFileName = Path.GetFileName(manga.CoverImage);
                if (!string.Equals(coverFileName, "no_cover.png", StringComparison.OrdinalIgnoreCase))
                {
                    string coverPath = Path.Combine(_webHostEnvironment.WebRootPath, manga.CoverImage.TrimStart('/'));
                    if (System.IO.File.Exists(coverPath))
                    {
                        System.IO.File.Delete(coverPath);
                    }
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

            // Xóa content
            var contents = _context.Content.Where(c => c.MangaId == id);
            _context.Content.RemoveRange(contents);

            // Xóa chapter
            var chapters = _context.Chapter.Where(ch => ch.MangaId == id);
            _context.Chapter.RemoveRange(chapters);

            // Xóa thư mục chứa ảnh truyện
            string mangaTitleSanitized = SanitizeFileName(manga.Title);
            string mangaFolder = Path.Combine(_webHostEnvironment.WebRootPath, "manga", $"{manga.MangaId} - {mangaTitleSanitized}");
            if (Directory.Exists(mangaFolder))
            {
                Directory.Delete(mangaFolder, true); // Xóa cả thư mục và tất cả file con
            }

            // Xóa bridge thể loại
            var mangaGenres = _context.Bridge_Manga_Genre.Where(bg => bg.MangaId == id);
            _context.Bridge_Manga_Genre.RemoveRange(mangaGenres);

            // Xóa bridge tác giả
            var mangaAuthors = _context.Bridge_Manga_Author.Where(ba => ba.MangaId == id);
            _context.Bridge_Manga_Author.RemoveRange(mangaAuthors);

            // Xóa manga
            _context.Manga.Remove(manga);

            await _context.SaveChangesAsync();

            return Ok();
        }


        // =================================================================================
        // ======================    THÊM CHAPTER CHO TRUYỆN  ==============================
        // =================================================================================
        public async Task<IActionResult> Upload(int id)
        {
            var manga = await _context.Manga.FindAsync(id);
            if (manga == null) return NotFound();

            var viewModel = new MangaChapterContentViewModel
            {
                MangaModel = manga,
                ChapterModels = _context.Chapter.Where(c => c.MangaId == id).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CreateChapter(int id)
        {
            var manga = await _context.Manga.FindAsync(id);
            if (manga == null) return NotFound();

            var viewModel = new MangaChapterContentViewModel
            {
                MangaModel = manga,
                ChapterModels = _context.Chapter.Where(c => c.MangaId == id).ToList()
            };

            return View(viewModel);
        }


        private string SanitizeFileName(string input)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                input = input.Replace(c, '_');
            }
            return input.Trim();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChapter(int MangaId, string Title, string Alias, bool IsPaid, decimal Price, List<IFormFile> Files)
        {
            if (Files == null || !Files.Any())
            {
                ModelState.AddModelError("", "Bạn cần chọn ít nhất một ảnh để tạo chapter.");
                return RedirectToAction("CreateChapter", new { id = MangaId });
            }

            var manga = await _context.Manga.FindAsync(MangaId);
            if (manga == null)
                return NotFound();

            // Nếu không trả phí, giá luôn bằng 0
            if (!IsPaid)
            {
                Price = 0;
            }

            // 1. Tạo Chapter mới
            var chapter = new ChapterModel
            {
                MangaId = MangaId,
                Title = Title,
                Alias = Alias,
                Price = Price,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chapter.Add(chapter);
            await _context.SaveChangesAsync();

            // Tạo thư mục lưu ảnh
            string mangaTitleSanitized = SanitizeFileName(manga.Title);
            string chapterTitleSanitized = SanitizeFileName(Title);
            string chapterPath = Path.Combine("wwwroot", "manga", $"{MangaId} - {mangaTitleSanitized}", chapterTitleSanitized);

            if (!Directory.Exists(chapterPath))
                Directory.CreateDirectory(chapterPath);

            // Sắp xếp Files theo thứ tự số học từ tên gốc
            var orderedFiles = Files.OrderBy(f =>
            {
                var match = Regex.Match(f.FileName, @"\d+");
                return match.Success ? int.Parse(match.Value) : int.MaxValue;
            }).ToList();

            int index = 1;
            foreach (var file in orderedFiles)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                var fileName = $"{index}{extension}";
                var filePath = Path.Combine(chapterPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Lưu Content vào DB
                var content = new ContentModel
                {
                    MangaId = MangaId,
                    ChapterId = chapter.ChapterId,
                    ContentNum = index,
                    Image = Path.Combine("manga", $"{MangaId} - {mangaTitleSanitized}", chapterTitleSanitized, fileName).Replace("\\", "/")
                };

                _context.Content.Add(content);
                index++;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Upload", new { id = MangaId });
        }

        // =================================================================================
        // =======================    CHỈNH CHAPTER TRUYỆN  ================================
        // =================================================================================
        public async Task<IActionResult> UpdateChapter(int mangaId, int chapterId)
        {
            var manga = await _context.Manga.FindAsync(mangaId);
            if (manga == null) return NotFound();

            var chapter = await _context.Chapter.FindAsync(chapterId);
            if (chapter == null || chapter.MangaId != mangaId) return NotFound();

            var viewModel = new MangaChapterContentViewModel
            {
                MangaModel = manga,
                ChapterModel = chapter, // Chương cần chỉnh sửa
                ChapterModels = _context.Chapter
                    .Where(c => c.ChapterId == chapterId)
                    .OrderBy(c => c.CreatedAt)
                    .ToList(),
                ContentModels = await _context.Content
                    .Where(c => c.ChapterId == chapterId)
                    .OrderBy(c => c.ContentNum)
                    .ToListAsync()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateChapter(int MangaId, int ChapterId, string Title, string Alias, bool IsPaid, decimal Price, List<IFormFile> Files)
        {
            bool hasNewFiles = Files != null && Files.Any();

            // Tìm Manga theo ID
            var manga = await _context.Manga.FindAsync(MangaId);
            if (manga == null)
                return NotFound();

            // Tìm Chapter theo ID
            var chapter = await _context.Chapter.FindAsync(ChapterId);
            if (chapter == null || chapter.MangaId != MangaId)
                return NotFound();

            // Cập nhật thông tin chapter
            chapter.Title = Title;
            chapter.Alias = Alias;
            chapter.Price = IsPaid ? Price : 0;

            _context.Chapter.Update(chapter);

            if (hasNewFiles)
            {
                // Xóa toàn bộ Content cũ
                var oldContents = _context.Content.Where(c => c.ChapterId == ChapterId).ToList();
                _context.Content.RemoveRange(oldContents);

                // Xóa thư mục chứa ảnh cũ
                string mangaTitleSanitized = SanitizeFileName(manga.Title);
                string oldChapterTitleSanitized = SanitizeFileName(chapter.Title); // đã cập nhật Title ở trên
                string oldChapterPath = Path.Combine("wwwroot", "manga", $"{MangaId} - {mangaTitleSanitized}", oldChapterTitleSanitized);

                if (Directory.Exists(oldChapterPath))
                {
                    Directory.Delete(oldChapterPath, true);
                }

                // Tạo lại thư mục lưu ảnh
                string newChapterTitleSanitized = SanitizeFileName(Title);
                string chapterPath = Path.Combine("wwwroot", "manga", $"{MangaId} - {mangaTitleSanitized}", newChapterTitleSanitized);

                if (!Directory.Exists(chapterPath))
                    Directory.CreateDirectory(chapterPath);

                // Sắp xếp file theo số trong tên ảnh
                var orderedFiles = Files.OrderBy(f =>
                {
                    var match = Regex.Match(f.FileName, @"\d+");
                    return match.Success ? int.Parse(match.Value) : int.MaxValue;
                }).ToList();

                int index = 1;
                foreach (var file in orderedFiles)
                {
                    var extension = Path.GetExtension(file.FileName).ToLower();
                    var fileName = $"{index}{extension}";
                    var filePath = Path.Combine(chapterPath, fileName);

                    // Lưu ảnh mới
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Tạo mới ContentModel
                    var content = new ContentModel
                    {
                        MangaId = MangaId,
                        ChapterId = ChapterId,
                        ContentNum = index,
                        Image = Path.Combine("manga", $"{MangaId} - {mangaTitleSanitized}", newChapterTitleSanitized, fileName).Replace("\\", "/")
                    };

                    _context.Content.Add(content);
                    index++;
                }
            }

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return RedirectToAction("Upload", new { id = MangaId });
        }

        // =================================================================================
        // ========================    XÓA CHAPTER TRUYỆN  =================================
        // =================================================================================
        [HttpGet]
        public async Task<IActionResult> DeleteChapter(int chapterId)
        {
            var chapter = await _context.Chapter.FindAsync(chapterId);
            if (chapter == null) return NotFound();

            var contents = _context.Content.Where(c => c.ChapterId == chapterId);
            _context.Content.RemoveRange(contents);

            // Xóa thư mục vật lý chứa ảnh
            var manga = await _context.Manga.FindAsync(chapter.MangaId);
            if (manga != null)
            {
                string mangaTitleSanitized = SanitizeFileName(manga.Title);
                string chapterTitleSanitized = SanitizeFileName(chapter.Title);
                string chapterPath = Path.Combine("wwwroot", "manga", $"{manga.MangaId} - {mangaTitleSanitized}", chapterTitleSanitized);

                if (Directory.Exists(chapterPath))
                    Directory.Delete(chapterPath, true);
            }

            _context.Chapter.Remove(chapter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Upload", new { id = chapter.MangaId });
        }
    }
}
