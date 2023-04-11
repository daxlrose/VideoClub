namespace VideoClub.Data.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public Customer Customer { get; set; }
        public Movie Movie { get; set; }
    }
}
