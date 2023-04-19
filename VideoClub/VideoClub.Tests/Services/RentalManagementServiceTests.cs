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
        private readonly IRentalManagementService _rentalManagementService;

        public RentalManagementServiceTests()
        {
            var options = new DbContextOptionsBuilder<VideoClubDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new VideoClubDbContext(options);
            _rentalManagementService = new RentalManagementService(_dbContext);
        }

        [Fact]
        public async Task AddRentalAsync_AddsNewRentalAndDecrementsMovieAvailableStock()
        {
            // Arrange
            var movie = GetSampleMovies().First();
            _dbContext.Movies.Add(movie);
            await _dbContext.SaveChangesAsync();

            var rental = new Rental
            {
                MovieId = movie.Id,
                UserId = "testUserId",
                RentalDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7)
            };

            // Act
            var addedRental = await _rentalManagementService.AddRentalAsync(rental);

            // Assert
            Assert.NotNull(addedRental);
            Assert.Equal(movie.Id, addedRental.MovieId);
            Assert.Equal(movie.AvailableStock, addedRental.Movie.AvailableStock);
        }

        [Fact]
        public async Task AddRentalAsync_ThrowsInvalidOperationException_WhenMovieOutOfStock()
        {
            // Arrange
            var movie = GetSampleMovies().First();
            movie.AvailableStock = 0;
            _dbContext.Movies.Add(movie);
            await _dbContext.SaveChangesAsync();

            var rental = new Rental
            {
                MovieId = movie.Id,
                UserId = "testUserId",
                RentalDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7)
            };

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _rentalManagementService.AddRentalAsync(rental));
        }

        [Fact]
        public async Task GetRentalByIdAsync_ReturnsRental_WhenRentalExists()
        {
            // Arrange
            var newRental = GetSampleRentals().First();
            _dbContext.Rentals.Add(newRental);
            await _dbContext.SaveChangesAsync();

            // Act
            var foundRental = await _rentalManagementService.GetRentalByIdAsync(newRental.Id);

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
            var foundRental = await _rentalManagementService.GetRentalByIdAsync(nonExistentRentalId);

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
            var overdueRentals = await _rentalManagementService.GetOverdueRentalsAsync();

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

        private List<Movie> GetSampleMovies()
        {
            return new List<Movie>
        {
            new Movie
            {
                Id = 1,
                Title = "Sample Movie 1",
                Description = "Sample",
                Director = "Sample",
                AvailableStock = 5
            },
            new Movie
            {
                Id = 2,
                Title = "Sample Movie 2",
                Description = "Sample",
                Director = "Sample",
                AvailableStock = 3
            }
        };
        }

        private List<Rental> GetSampleRentals()
        {
            var user = new ApplicationUser { Id = Guid.NewGuid().ToString() };
            var movie = GetSampleMovies().First();

            var rentals = new List<Rental>
        {
            new Rental
            {
                Id = 1,
                MovieId = movie.Id,
                UserId = user.Id,
                User = user,
                RentalDate = DateTime.Now.AddDays(-10),
                DueDate = DateTime.Now.AddDays(-5),
                Returned = false
            },
            new Rental
            {
                Id = 2,
                MovieId = movie.Id,
                UserId = user.Id,
                User = user,
                RentalDate = DateTime.Now.AddDays(-7),
                DueDate = DateTime.Now.AddDays(-3),
                Returned = false
            },
            new Rental
            {
                Id = 3,
                MovieId = movie.Id,
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
