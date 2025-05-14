using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebTruyenTranh.Data;
using WebTruyenTranh.Models;
using WebTruyenTranh.Models.ViewModel;
namespace WebTruyenTranh.Controllers
{
    public class MangaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public MangaController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public async Task<IActionResult> DetailManga(int MangaId)
        {
            var manga = await _context.Manga.FindAsync(MangaId);
            if (manga == null)
                return NotFound();
            var chapters = await _context.Chapter
                .Where(c => c.MangaId == MangaId)
                .OrderBy(c => c.Title)
                .ToListAsync();
            var mangaGenres = await _context.Bridge_Manga_Genre
                .Where(bg => bg.MangaId == MangaId)
                .ToListAsync();
            var mangaAuthors = await _context.Bridge_Manga_Author
                .Where(ba => ba.MangaId == MangaId)
                .ToListAsync();
            var genreIds = mangaGenres.Select(bg => bg.GenreId).Distinct().ToList();
            var authorIds = mangaAuthors.Select(ba => ba.AuthorId).Distinct().ToList();
            var genres = await _context.Genre
                .Where(g => genreIds.Contains(g.GenreId))
                .ToListAsync();
            var authors = await _context.Author
                .Where(a => authorIds.Contains(a.AuthorId))
                .ToListAsync();
            var viewModel = new MangaDetailViewModel
            {
                MangaModel = manga,
                ChaptersModels = chapters,
                GenresModels = genres,
                AuthorsModels = authors
            };
            return View(viewModel);
        }

        // Lấy thông tin đầy đủ gồm Genre, Author cho danh sách manga được đưa vào
        private List<MangaAuthorGenreViewModel> GetMangaWithDetails(List<MangaModel> mangas)
        {
            var mangaIds = mangas.Select(m => m.MangaId).ToList();

            var mangaGenres = _context.Bridge_Manga_Genre
                                      .Where(bg => mangaIds.Contains(bg.MangaId))
                                      .ToList();

            var mangaAuthors = _context.Bridge_Manga_Author
                                       .Where(ba => mangaIds.Contains(ba.MangaId))
                                       .ToList();

            var genreIds = mangaGenres.Select(bg => bg.GenreId).Distinct().ToList();
            var authorIds = mangaAuthors.Select(ba => ba.AuthorId).Distinct().ToList();

            var genres = _context.Genre
                                 .Where(g => genreIds.Contains(g.GenreId))
                                 .ToList();

            var authors = _context.Author
                                  .Where(a => authorIds.Contains(a.AuthorId))
                                  .ToList();

            var result = new List<MangaAuthorGenreViewModel>();

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

                result.Add(new MangaAuthorGenreViewModel
                {
                    MangaModel = manga,
                    GenresModels = relatedGenres,
                    AuthorsModels = relatedAuthors
                });
            }

            return result;
        }

        public async Task<IActionResult> ReadManga(int MangaId, int ChapterId)
        {
            var manga = await _context.Manga.FindAsync(MangaId);
            if (manga == null)
                return NotFound();

            var chapters = await _context.Chapter
                .Where(c => c.MangaId == MangaId)
                .OrderBy(c => c.Title)
                .ToListAsync();

            var chapter = await _context.Chapter
                .FirstOrDefaultAsync(c => c.ChapterId == ChapterId && c.MangaId == MangaId);
            if (chapter == null)
                return NotFound();

            // Chapter trước: Title “nhỏ hơn”, sắp xếp giảm dần, lấy bản ghi đầu tiên
            var prevChapter = await _context.Chapter
                .Where(c => c.MangaId == MangaId
                            && string.Compare(c.Title, chapter.Title) < 0)
                .OrderByDescending(c => c.Title)
                .FirstOrDefaultAsync();

            // Chapter kế tiếp: Title “lớn hơn”, sắp xếp tăng dần, lấy bản ghi đầu tiên
            var nextChapter = await _context.Chapter
                .Where(c => c.MangaId == MangaId
                            && string.Compare(c.Title, chapter.Title) > 0)
                .OrderBy(c => c.Title)
                .FirstOrDefaultAsync();

            var contents = await _context.Content
                .Where(c => c.ChapterId == ChapterId)
                .OrderBy(c => c.ContentNum)
                .ToListAsync();

            //var comments = await _context.Comment
            //    .Where(c => c.MangaId == MangaId && c.ChapterId == ChapterId)
            //    .OrderByDescending(c => c.CreatedAt)
            //    .ToListAsync();

            var viewModel = new MangaPageViewModel
            {
                Manga = manga,
                Chapters = chapters,
                Chapter = chapter,
                Contents = contents,
                PrevChapter = prevChapter,
                NextChapter = nextChapter
                //Comments = comments
            };

            return View(viewModel);
        }
    }
}