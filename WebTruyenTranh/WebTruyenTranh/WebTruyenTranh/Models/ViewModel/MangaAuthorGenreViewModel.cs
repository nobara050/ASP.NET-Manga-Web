using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models.ViewModel
{
    public class MangaAuthorGenreViewModel
    {
        public MangaModel MangaModel { get; set; }
        public List<GenreModel>? GenresModels { get; set; } 
        public List<AuthorModel>? AuthorsModels { get; set; }
        public List<GenreModel>? GenresModelAll { get; set; }
        public List<AuthorModel>? AuthorsModelAll { get; set; }
        public List<int>? SelectedGenreIds { get; set; }
        public List<String>? ListAuthorName { get; set; }
    }
}
