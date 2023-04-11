using Microsoft.EntityFrameworkCore;
using VideoClub.Data.Models;

namespace VideoClub.Data
{
    public class VideoClubDbContext : DbContext
    {
        public VideoClubDbContext(DbContextOptions<VideoClubDbContext> options)
       : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
