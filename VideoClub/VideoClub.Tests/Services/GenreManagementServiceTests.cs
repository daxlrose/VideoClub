using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
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

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
