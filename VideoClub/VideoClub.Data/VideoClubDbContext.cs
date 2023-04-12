using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VideoClub.Data.Models;

namespace VideoClub.Data
{
    public class VideoClubDbContext : IdentityDbContext<ApplicationUser>
    {
        public VideoClubDbContext(DbContextOptions<VideoClubDbContext> options)
       : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Rental> Rentals { get; set; }
    }
}
