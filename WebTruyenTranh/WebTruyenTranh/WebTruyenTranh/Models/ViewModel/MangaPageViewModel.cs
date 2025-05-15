using System.ComponentModel.DataAnnotations;

namespace WebTruyenTranh.Models.ViewModel
{
    public class MangaPageViewModel
    {
        public MangaModel? Manga { get; set; }
        public List<ChapterModel>? Chapters { get; set; }
        public ChapterModel? Chapter { get; set; }
        public List<ContentModel>? Contents { get; set; }
        public ChapterModel? NextChapter { get; set; }
        public ChapterModel? PrevChapter { get; set; }
        public List<CommentModel>? Comments { get; set; }
    }
}
