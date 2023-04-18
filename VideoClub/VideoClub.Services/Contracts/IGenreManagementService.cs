using VideoClub.Data.Models;

namespace VideoClub.Services.Contracts
{
    public interface IGenreManagementService
    {
        Task<Genre> AddGenreAsync(Genre genre);
        Task<Genre> GetGenreByIdAsync(int id);
        Task UpdateGenreAsync(Genre genre);
        Task DeleteGenreAsync(Genre genre);
        Task<IEnumerable<Genre>> GetAllGenresAsync();
    }
}
