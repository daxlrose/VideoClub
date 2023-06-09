﻿using Microsoft.EntityFrameworkCore;
using VideoClub.Data;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;

namespace VideoClub.Services.Implementations
{
    public class MovieManagementService : IMovieManagementService
    {
        private readonly VideoClubDbContext _dbContext;

        public MovieManagementService(VideoClubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            _dbContext.Movies.Add(movie);
            await _dbContext.SaveChangesAsync();
            return movie;
        }

        public async Task DeleteMovieAsync(Movie movie)
        {
            _dbContext.Movies.Remove(movie);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _dbContext.Movies.Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre).ToListAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _dbContext.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            _dbContext.Movies.Update(movie);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genre)
        {
            return await _dbContext.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(m => m.MovieGenres.Any(mg => mg.Genre.Name == genre))
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Movie>();
            }

            var searchQuery = query.Trim().ToLowerInvariant();

            return await _dbContext.Movies
            .Where(m => EF.Functions.Like(m.Title.ToLower(), $"%{searchQuery}%"))
            .ToListAsync();
        }
    }
}
