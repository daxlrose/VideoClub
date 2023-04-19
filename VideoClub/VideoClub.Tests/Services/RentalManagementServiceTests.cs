using Microsoft.EntityFrameworkCore;
using VideoClub.Data;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;
using VideoClub.Services.Implementations;

namespace VideoClub.Tests.Services
{
    public class RentalManagementServiceTests : IDisposable
    {
        private readonly VideoClubDbContext _dbContext;
        private readonly IRentalManagementService _service;

        public RentalManagementServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoClubDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new VideoClubDbContext(options);
            _service = new RentalManagementService(_dbContext);
        }

        [Fact]
        public async Task AddRentalAsync_AddsNewRentalAndReturnsIt()
        {
            // Arrange
            var sampleRental = GetSampleRentals().First();

            // Act
            var addedRental = await _service.AddRentalAsync(sampleRental);

            // Assert
            Assert.NotNull(addedRental);
            Assert.Equal(sampleRental.MovieId, addedRental.MovieId);
        }

        [Fact]
        public async Task GetRentalByIdAsync_ReturnsRental_WhenRentalExists()
        {
            // Arrange
            var newRental = GetSampleRentals().First();
            _dbContext.Rentals.Add(newRental);
            await _dbContext.SaveChangesAsync();

            // Act
            var foundRental = await _service.GetRentalByIdAsync(newRental.Id);

            // Assert
            Assert.NotNull(foundRental);
            Assert.Equal(newRental.Id, foundRental.Id);
            Assert.Equal(newRental.MovieId, foundRental.MovieId);
        }

        [Fact]
        public async Task GetRentalByIdAsync_ReturnsNull_WhenRentalDoesNotExist()
        {
            // Arrange
            int nonExistentRentalId = 999;

            // Act
            var foundRental = await _service.GetRentalByIdAsync(nonExistentRentalId);

            // Assert
            Assert.Null(foundRental);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private List<Rental> GetSampleRentals()
        {
            var rentals = new List<Rental>()
            {
                new Rental
                {
                    MovieId = 1,
                    UserId = new Guid().ToString(),
                    RentalDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(7)
                },
                new Rental
                {
                    MovieId = 2,
                    UserId = new Guid().ToString(),
                    RentalDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(7)
                },
                new Rental
                {
                    MovieId = 3,
                    UserId = new Guid().ToString(),
                    RentalDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(7)
                }
            };

            return rentals;
        }
    }
}
