using VideoClub.Data.Models;

namespace VideoClub.Services.Contracts
{
    public interface IRentalManagementService
    {
        Task<Rental> AddRentalAsync(Rental rental);
        Task<Rental> GetRentalByIdAsync(int id);
    }
}
