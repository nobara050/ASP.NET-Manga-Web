CREATE DATABASE MangaDB;
GO
USE MangaDB;

TRUNCATE TABLE Author;
TRUNCATE TABLE Bridge_Manga_Author;
TRUNCATE TABLE Bridge_Manga_Genre;
TRUNCATE TABLE Chapter;
TRUNCATE TABLE Content;
TRUNCATE TABLE Genre;
TRUNCATE TABLE Manga;

CREATE TABLE Manga (
    MangaId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(255) NOT NULL,
    Description NVARCHAR(2000),
    CoverImage NVARCHAR(1000),
    BackgroundImage NVARCHAR(1000),
    Status NVARCHAR(50),
    UserId INT,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Chapter (
	ChapterId INT IDENTITY(1,1) PRIMARY KEY,
    MangaId INT,
    Title NVARCHAR(255) NOT NULL,
	Alias NVARCHAR(255),
    Price DECIMAL(10,2) DEFAULT 0 NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Content (
	ContentId INT IDENTITY(1,1) PRIMARY KEY,
    MangaId INT NOT NULL,
    ChapterId INT NOT NULL,
	ContentNum INT NOT NULL,
    Image NVARCHAR(1000) NOT NULL
);

CREATE TABLE Genre (
    GenreId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL
);

CREATE TABLE Author (
    AuthorId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL
);

CREATE TABLE Bridge_Manga_Genre (
    MangaId INT,
    GenreId INT,
    PRIMARY KEY (MangaId, GenreId)
);

CREATE TABLE Bridge_Manga_Author (
    MangaId INT,
    AuthorId INT,
    PRIMARY KEY (MangaId, AuthorId)
);
 
CREATE TABLE Account (
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    Avatar NVARCHAR(1000),
    Balance DECIMAL(10,2) NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

INSERT INTO Account (Username, Email, Password, Role)
VALUES ('admin', 'nguyentiendat050@gmail.com', 'admin', 'Admin');

SELECT * FROM Account; 
