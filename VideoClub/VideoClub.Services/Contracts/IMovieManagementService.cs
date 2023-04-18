using VideoClub.Data.Models;

namespace VideoClub.Services.Contracts
{
    public interface IMovieManagementService
    {
        Task<Movie> AddMovieAsync(Movie movie);
        Task<Movie> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(Movie movie);
    }
}
