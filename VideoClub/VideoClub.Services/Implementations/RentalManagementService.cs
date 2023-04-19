using Microsoft.EntityFrameworkCore;
using VideoClub.Data;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;

namespace VideoClub.Services.Implementations
{
    public class RentalManagementService : IRentalManagementService
    {
        private readonly VideoClubDbContext _dbContext;

        public RentalManagementService(VideoClubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Rental> AddRentalAsync(Rental rental)
        {
            _dbContext.Rentals.Add(rental);
            await _dbContext.SaveChangesAsync();
            return rental;
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            return await _dbContext.Rentals.FindAsync(id);
        }

        public async Task<IEnumerable<Rental>> GetOverdueRentalsAsync()
        {
            return await _dbContext.Rentals
                .Where(r => r.DueDate < DateTime.Now && !r.Returned)
                .ToListAsync();
        }
    }
}
