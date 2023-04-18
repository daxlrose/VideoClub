using System.ComponentModel.DataAnnotations;

namespace VideoClub.Common.DTOs.Genres
{
    public class UpdateGenreDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
