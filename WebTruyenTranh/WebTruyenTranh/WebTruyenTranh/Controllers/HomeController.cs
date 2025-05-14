using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebTruyenTranh.Data;
using WebTruyenTranh.Models;
using WebTruyenTranh.Models.ViewModel;

namespace WebTruyenTranh.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;
    public HomeController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
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

    // Hàm lấy danh sách manga mới nhất (top 12) => Mục đích để hiển thị trên Corousel
    private List<MangaAuthorGenreViewModel> GetTopNewMangas(int take = 12)
    {
        var mangas = _context.Manga
                             .OrderByDescending(m => m.CreatedAt)
                             .Take(take)
                             .ToList();

        return GetMangaWithDetails(mangas);
    }

    public IActionResult Index()
    {
        // Lấy danh sách truyện
        var pagedMangas = _context.Manga
                                  .Take(24)
                                  .ToList();

        var mangaListView = GetMangaWithDetails(pagedMangas);

        // Lấy top mới ra
        var topNewListView = GetTopNewMangas(12);

        var viewModel = new HomePageViewModel
        {
            MangaPagedList = mangaListView,
            TopNewCorousel = topNewListView
        };

        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
