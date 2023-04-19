namespace VideoClub.Common.DTOs.Rentals
{
    public class RentalDto
    {
        public int MovieId { get; set; }
        public string UserId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}
