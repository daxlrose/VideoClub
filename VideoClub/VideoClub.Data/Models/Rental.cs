namespace VideoClub.Data.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Returned { get; set; }

        public ApplicationUser User { get; set; }
        public Movie Movie { get; set; }
    }
}
