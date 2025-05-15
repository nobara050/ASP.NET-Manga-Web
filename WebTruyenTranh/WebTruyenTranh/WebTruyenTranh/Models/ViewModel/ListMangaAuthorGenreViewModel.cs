using WebTruyenTranh.Data;

namespace WebTruyenTranh.Models.ViewModel
{
    public class ListMangaAuthorGenreViewModel
    {
        public List<MangaAuthorGenreViewModel> Mangas { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public static ListMangaAuthorGenreViewModel GetPagedData(ApplicationDbContext context, int page, int pageSize = 10)
        {
            var totalItems = context.Manga.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var mangas = context.Manga
                                .OrderBy(m => m.MangaId)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            var mangaIds = mangas.Select(m => m.MangaId).ToList();

            var mangaGenres = context.Bridge_Manga_Genre
                                     .Where(bg => mangaIds.Contains(bg.MangaId))
                                     .ToList();

            var mangaAuthors = context.Bridge_Manga_Author
                                      .Where(ba => mangaIds.Contains(ba.MangaId))
                                      .ToList();

            var genreIds = mangaGenres.Select(bg => bg.GenreId).Distinct().ToList();
            var authorIds = mangaAuthors.Select(ba => ba.AuthorId).Distinct().ToList();

            var genres = context.Genre
                                .Where(g => genreIds.Contains(g.GenreId))
                                .ToList();

            var authors = context.Author
                                 .Where(a => authorIds.Contains(a.AuthorId))
                                 .ToList();

            var list = new List<MangaAuthorGenreViewModel>();

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

                list.Add(new MangaAuthorGenreViewModel
                {
                    MangaModel = manga,
                    GenresModels = relatedGenres,
                    AuthorsModels = relatedAuthors
                });
            }

            return new ListMangaAuthorGenreViewModel
            {
                Mangas = list,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }
    }

}
