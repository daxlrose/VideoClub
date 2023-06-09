﻿using Microsoft.EntityFrameworkCore;
using VideoClub.Data;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;
using VideoClub.Services.Implementations;

namespace VideoClub.Tests.Services
{
    public class MovieManagementServiceTests : IDisposable
    {
        private readonly VideoClubDbContext _dbContext;
        private readonly IMovieManagementService _service;

        public MovieManagementServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoClubDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new VideoClubDbContext(options);
            _service = new MovieManagementService(_dbContext);
        }

        [Fact]
        public async Task AddMovieAsync_AddsNewMovieAndReturnsIt()
        {
            // Arrange
            var sampleMovie = GetSampleMovies().First();

            // Act
            var addedMovie = await _service.AddMovieAsync(sampleMovie);

            // Assert
            Assert.NotNull(addedMovie);
            Assert.Equal(sampleMovie.Title, addedMovie.Title);
        }

        [Fact]
        public async Task GetMovieByIdAsync_ReturnsMovie_WhenMovieExists()
        {
            // Arrange
            var newMovie = GetSampleMovies().First();
            _dbContext.Movies.Add(newMovie);
            await _dbContext.SaveChangesAsync();

            // Act
            var foundMovie = await _service.GetMovieByIdAsync(newMovie.Id);

            // Assert
            Assert.NotNull(foundMovie);
            Assert.Equal(newMovie.Id, foundMovie.Id);
            Assert.Equal(newMovie.Title, foundMovie.Title);
        }

        [Fact]
        public async Task GetMovieByIdAsync_ReturnsNull_WhenMovieDoesNotExist()
        {
            // Arrange
            int nonExistentMovieId = 999;

            // Act
            var foundMovie = await _service.GetMovieByIdAsync(nonExistentMovieId);

            // Assert
            Assert.Null(foundMovie);
        }

        [Fact]
        public async Task UpdateMovieAsync_UpdatesExistingMovie()
        {
            // Arrange
            var newMovie = GetSampleMovies().First();
            _dbContext.Movies.Add(newMovie);
            await _dbContext.SaveChangesAsync();

            var updatedMovieTitle = "Title updated";

            // Act
            var existingMovie = await _dbContext.Movies.FindAsync(newMovie.Id);
            existingMovie.Title = updatedMovieTitle;
            await _service.UpdateMovieAsync(existingMovie);

            // Assert
            var foundMovie = await _dbContext.Movies.FindAsync(newMovie.Id);
            Assert.NotNull(foundMovie);
            Assert.Equal(updatedMovieTitle, foundMovie.Title);
        }

        [Fact]
        public async Task DeleteMovieAsync_RemovesExistingMovie()
        {
            // Arrange
            var newMovie = GetSampleMovies().First();
            _dbContext.Movies.Add(newMovie);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.DeleteMovieAsync(newMovie);

            // Assert
            var foundMovie = await _dbContext.Movies.FindAsync(newMovie.Id);
            Assert.Null(foundMovie);
        }

        [Fact]
        public async Task GetAllMoviesAsync_ReturnsAllMovies()
        {
            // Arrange
            var movies = GetSampleMovies();

            _dbContext.Movies.AddRange(movies);
            await _dbContext.SaveChangesAsync();

            // Act
            var allMovies = await _service.GetAllMoviesAsync();

            // Assert
            Assert.NotNull(allMovies);
            Assert.Equal(3, allMovies.Count());
            Assert.Contains(allMovies, m => m.Title == "Test Movie");
            Assert.Contains(allMovies, m => m.Title == "Test Movie2");
            Assert.Contains(allMovies, m => m.Title == "Test Movie3");
        }

        [Fact]
        public async Task GetMoviesByGenreAsync_ReturnsMoviesWithProvidedGenre()
        {
            // Arrange
            var movies = GetSampleMovies();

            _dbContext.Movies.AddRange(movies);
            await _dbContext.SaveChangesAsync();

            // Act
            var moviesByGenre = await _service.GetMoviesByGenreAsync("Action");

            // Assert
            Assert.NotNull(moviesByGenre);
            Assert.Equal(2, moviesByGenre.Count());
            Assert.Contains(moviesByGenre, m => m.Title == "Test Movie");
            Assert.Contains(moviesByGenre, m => m.Title == "Test Movie2");
        }

        [Theory]
        [InlineData("test movie2", 1)]
        [InlineData("movie", 3)]
        [InlineData("nonexistent", 0)]
        public async Task SearchMoviesAsync_ReturnsMatchingMovies(string query, int expectedCount)
        {
            // Arrange
            var movies = GetSampleMovies();

            _dbContext.Movies.AddRange(movies);
            await _dbContext.SaveChangesAsync();

            // Act
            var searchResults = await _service.SearchMoviesAsync(query);

            // Assert
            Assert.NotNull(searchResults);
            Assert.Equal(expectedCount, searchResults.Count());
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private List<Movie> GetSampleMovies()
        {
            var genre1 = new Genre { Name = "Action" };
            var genre2 = new Genre { Name = "Comedy" };

            var movies = new List<Movie>()
            {
                new Movie
                {
                    Title = "Test Movie",
                    Description = "Test description",
                    ReleaseDate = new DateTime(2010, 7, 16),
                    Director = "Test director",
                    DurationInMinutes = 60,
                    TotalStock = 10,
                    AvailableStock = 10,
                    RentalPrice = 7,
                    MovieGenres =new List<MovieGenre> { new MovieGenre { Genre = genre1 } }
                },
                new Movie
                {
                    Title = "Test Movie2",
                    Description = "Test description2",
                    ReleaseDate = new DateTime(2010, 7, 16),
                    Director = "Test director2",
                    DurationInMinutes = 60,
                    TotalStock = 10,
                    AvailableStock = 10,
                    RentalPrice = 7,
                    MovieGenres =new List<MovieGenre> { new MovieGenre { Genre = genre1 } }
                },
                new Movie
                {
                    Title = "Test Movie3",
                    Description = "Test description3",
                    ReleaseDate = new DateTime(2010, 7, 16),
                    Director = "Test director3",
                    DurationInMinutes = 60,
                    TotalStock = 10,
                    AvailableStock = 10,
                    RentalPrice = 7,
                    MovieGenres =new List<MovieGenre> { new MovieGenre { Genre = genre2 } }
                }
            };

            return movies;
        }
    }
}
