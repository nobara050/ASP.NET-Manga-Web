﻿using Microsoft.EntityFrameworkCore;
using WebTruyenTranh.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace WebTruyenTranh.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // Khai báo các DbSet tương ứng với các bảng trong cơ sở dữ liệu
        public DbSet<Bridge_Manga_GenreModel> Bridge_Manga_Genre { get; set; }
        public DbSet<Bridge_Manga_AuthorModel> Bridge_Manga_Author { get; set; }
        public DbSet<AuthorModel> Author { get; set; }
        public DbSet<GenreModel> Genre { get; set; }
        public DbSet<MangaModel> Manga { get; set; }
        public DbSet<AccountModel> Account { get; set; }
        public DbSet<ChapterModel> Chapter { get; set; }
        public DbSet<ContentModel> Content { get; set; }
        public DbSet<CommentModel> Comment { get; set; }
        public DbSet<CommentLikeModel> CommentLike { get; set; }



        // Khai báo với EF rằng sẽ ánh xạ tên của Entity với tên bảng trong cơ sở dữ liệu
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorModel>().ToTable("Author");
            modelBuilder.Entity<GenreModel>().ToTable("Genre");
            modelBuilder.Entity<MangaModel>().ToTable("Manga");
            modelBuilder.Entity<AccountModel>().ToTable("Account");
            modelBuilder.Entity<ChapterModel>().ToTable("Chapter");
            modelBuilder.Entity<ContentModel>().ToTable("Content");
            modelBuilder.Entity<CommentModel>().ToTable("Comment");
            modelBuilder.Entity<CommentLikeModel>().ToTable("CommentLike");
            modelBuilder.Entity<Bridge_Manga_AuthorModel>()
            .HasKey(x => new { x.MangaId, x.AuthorId });
            modelBuilder.Entity<Bridge_Manga_GenreModel>()
            .HasKey(x => new { x.MangaId, x.GenreId });
        }
    }
}
