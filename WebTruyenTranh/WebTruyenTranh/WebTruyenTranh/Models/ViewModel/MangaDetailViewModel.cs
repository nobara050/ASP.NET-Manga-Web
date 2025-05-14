namespace WebTruyenTranh.Models.ViewModel
{
    public class MangaDetailViewModel
    {
        public MangaModel MangaModel { get; set; }
        public List<ChapterModel> ChaptersModels { get; set; }
        public List<GenreModel> GenresModels { get; set; }
        public List<AuthorModel> AuthorsModels { get; set; }
        //public List<CommentModel> Comments { get; set; } 
    }
}
