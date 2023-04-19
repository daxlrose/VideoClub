namespace VideoClub.Common.DTOs.Rentals
{
    public class CreateRentalDto
    {
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
