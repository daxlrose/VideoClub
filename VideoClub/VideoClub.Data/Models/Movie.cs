namespace VideoClub.Data.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int DurationInMinutes { get; set; }
        public string Genre { get; set; }
        public int TotalStock { get; set; }
        public int AvailableStock { get; set; }
        public decimal RentalPrice { get; set; }
    }
}
