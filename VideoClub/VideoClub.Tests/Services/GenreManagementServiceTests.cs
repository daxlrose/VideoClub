using Microsoft.EntityFrameworkCore;
using VideoClub.Data;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;
using VideoClub.Services.Implementations;

namespace VideoClub.Tests.Services
{
    public class GenreManagementServiceTests : IDisposable
    {
        private readonly VideoClubDbContext _dbContext;
        private readonly IGenreManagementService _service;

        public GenreManagementServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoClubDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new VideoClubDbContext(options);
            _service = new GenreManagementService(_dbContext);
        }

        [Fact]
        public async Task AddGenreAsync_AddsNewGenreAndReturnsIt()
        {
            // Arrange
            var newGenre = new Genre { Name = "Action" };

            // Act
            var addedGenre = await _service.AddGenreAsync(newGenre);

            // Assert
            Assert.NotNull(addedGenre);
            Assert.Equal(newGenre.Name, addedGenre.Name);
        }

        [Fact]
        public async Task GetGenreByIdAsync_ReturnsGenre_WhenGenreExists()
        {
            // Arrange
            var newGenre = new Genre { Name = "Adventure" };
            _dbContext.Genres.Add(newGenre);
            await _dbContext.SaveChangesAsync();

            // Act
            var foundGenre = await _service.GetGenreByIdAsync(newGenre.Id);

            // Assert
            Assert.NotNull(foundGenre);
            Assert.Equal(newGenre.Id, foundGenre.Id);
            Assert.Equal(newGenre.Name, foundGenre.Name);
        }

        [Fact]
        public async Task GetGenreByIdAsync_ReturnsNull_WhenGenreDoesNotExist()
        {
            // Arrange
            int nonExistentGenreId = 999;

            // Act
            var foundGenre = await _service.GetGenreByIdAsync(nonExistentGenreId);

            // Assert
            Assert.Null(foundGenre);
        }

        [Fact]
        public async Task UpdateGenreAsync_UpdatesExistingGenre()
        {
            // Arrange
            var newGenre = new Genre { Name = "Action" };
            _dbContext.Genres.Add(newGenre);
            await _dbContext.SaveChangesAsync();

            var updatedGenreName = "Updated Action";

            // Act
            var existingGenre = await _dbContext.Genres.FindAsync(newGenre.Id);
            existingGenre.Name = updatedGenreName;
            await _service.UpdateGenreAsync(existingGenre);

            // Assert
            var foundGenre = await _dbContext.Genres.FindAsync(newGenre.Id);
            Assert.NotNull(foundGenre);
            Assert.Equal(updatedGenreName, foundGenre.Name);
        }

        [Fact]
        public async Task DeleteGenreAsync_RemovesExistingGenre()
        {
            // Arrange
            var newGenre = new Genre { Name = "Action" };
            _dbContext.Genres.Add(newGenre);
            await _dbContext.SaveChangesAsync();

            // Act
            await _service.DeleteGenreAsync(newGenre);

            // Assert
            var foundGenre = await _dbContext.Genres.FindAsync(newGenre.Id);
            Assert.Null(foundGenre);
        }

        [Fact]
        public async Task GetAllGenresAsync_ReturnsAllGenres()
        {
            // Arrange
            var genre1 = new Genre { Name = "Action" };
            var genre2 = new Genre { Name = "Comedy" };
            var genre3 = new Genre { Name = "Drama" };

            _dbContext.Genres.AddRange(genre1, genre2, genre3);
            await _dbContext.SaveChangesAsync();

            // Act
            var allGenres = await _service.GetAllGenresAsync();

            // Assert
            Assert.NotNull(allGenres);
            Assert.Equal(3, allGenres.Count());
            Assert.Contains(allGenres, g => g.Name == "Action");
            Assert.Contains(allGenres, g => g.Name == "Comedy");
            Assert.Contains(allGenres, g => g.Name == "Drama");
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
