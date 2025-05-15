using Microsoft.AspNetCore.Authorization;
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
                .OrderByDescending(c => c.Title)
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

            var likeCount = await _context.CommentLike
                    .CountAsync(cl => _context.Comment
                        .Where(c => c.MangaId == MangaId && !c.IsDeleted)
                        .Select(c => c.CommentId)
                        .Contains(cl.CommentId));

            var viewModel = new MangaDetailViewModel
            {
                MangaModel = manga,
                ChaptersModels = chapters,
                GenresModels = genres,
                AuthorsModels = authors,
                Like = likeCount
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ReadManga(int MangaId, int ChapterId)
        {
            try
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

                // ========================================================================

                // Lấy danh sách các bình luận gốc (không phải reply)
                var comments = await _context.Comment
                    .Where(c => c.MangaId == MangaId &&
                               c.ChapterId == ChapterId &&
                               c.ParentCommentId == null &&
                               !c.IsDeleted)
                    .ToListAsync();

                // Lấy thông tin user và số lượng like cho mỗi comment
                foreach (var comment in comments)
                {
                    comment.LoadUser(_context);
                    comment.GetLikeCount(_context);
                }

                // Lấy các replies cho mỗi comment
                foreach (var comment in comments)
                {
                    var replies = await _context.Comment
                        .Where(c => c.ParentCommentId == comment.CommentId && !c.IsDeleted)
                        .ToListAsync();

                    foreach (var reply in replies)
                    {
                        reply.LoadUser(_context);
                        reply.GetLikeCount(_context);
                    }

                    comment.Replies = replies;
                    comment.SortReplies();
                }

                // Sắp xếp comments theo thời gian
                comments = comments.OrderByDescending(c => c.CreatedAt).ToList();
                
                var viewModel = new MangaPageViewModel
                {
                    Manga = manga,
                    Chapters = chapters,
                    Chapter = chapter,
                    Contents = contents,
                    PrevChapter = prevChapter,
                    NextChapter = nextChapter,
                    Comments = comments
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}