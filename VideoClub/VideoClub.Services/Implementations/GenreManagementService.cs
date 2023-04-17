﻿using VideoClub.Data;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;

namespace VideoClub.Services.Implementations
{
    public class GenreManagementService : IGenreManagementService
    {
        private readonly VideoClubDbContext _dbContext;

        public GenreManagementService(VideoClubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Genre> AddGenreAsync(Genre genre)
        {
            _dbContext.Genres.Add(genre);
            await _dbContext.SaveChangesAsync();
            return genre;
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            return await _dbContext.Genres.FindAsync(id);
        }
    }
}