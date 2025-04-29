namespace WebTruyenTranh.Models.ViewModel
{
    public class MangaChapterContentViewModel
    {
        public MangaModel MangaModel { get; set; }
        public ChapterModel? ChapterModel { get; set; }

        public List<ChapterModel>? ChapterModels { get; set; }
        public List<ContentModel>? ContentModels { get; set; }
    }
}
