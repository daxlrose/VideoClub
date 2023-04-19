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

        [Fact]
        public async Task GetOverdueRentalsAsync_ReturnsOverdueRentals()
        {
            // Arrange
            var rentals = GetSampleRentals();

            _dbContext.Rentals.AddRange(rentals);
            await _dbContext.SaveChangesAsync();

            // Act
            var overdueRentals = await _service.GetOverdueRentalsAsync();

            // Assert
            Assert.NotNull(overdueRentals);
            Assert.Equal(2, overdueRentals.Count());
            Assert.Contains(overdueRentals, r => r.Id == 1);
            Assert.Contains(overdueRentals, r => r.Id == 2);
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }



        private List<Rental> GetSampleRentals()
        {
            var user = new ApplicationUser { Id = new Guid().ToString() };

            var rentals = new List<Rental>()
    {
        new Rental
        {
            Id = 1,
            MovieId = 1,
            UserId = user.Id,
            User = user,
            RentalDate = DateTime.Now.AddDays(-10),
            DueDate = DateTime.Now.AddDays(-5),
            Returned = false
        },
        new Rental
        {
            Id = 2,
            MovieId = 1,
            UserId = user.Id,
            User = user,
            RentalDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(-3),
            Returned = false
        },
        new Rental
        {
            Id = 3,
            MovieId = 3,
            UserId = user.Id,
            User = user,
            RentalDate = DateTime.Now.AddDays(-3),
            DueDate = DateTime.Now.AddDays(3),
            Returned = false
        }
    };

            return rentals;
        }
    }
}
