using VideoClub.Common.DTOs.Genres;

namespace VideoClub.Common.DTOs.Movies
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DurationInMinutes { get; set; }
        public List<string> Genres { get; set; }
        public int TotalStock { get; set; }
        public int AvailableStock { get; set; }
        public decimal RentalPrice { get; set; }
    }
}
