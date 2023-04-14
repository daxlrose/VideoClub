using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoClub.Common.DTOs.Genres;
using VideoClub.Common.DTOs.Movies;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;

namespace VideoClub.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing genre-related actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreManagementService _genreManagementService;
        private readonly IMapper _mapper;

        public GenresController(IGenreManagementService genreManagementService, IMapper mapper)
        {
            _genreManagementService = genreManagementService;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new genre.
        /// </summary>
        /// <param name="genreDto">The genre to add.</param>
        /// <returns>A newly created genre.</returns>
        /// <response code="201">A new genre was created.</response>
        /// <response code="400">Invalid input.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GenreDto>> AddGenre([FromBody] CreateGenreDto genreDto)
        {
            var genre = _mapper.Map<Genre>(genreDto);
            var addedGenre = await _genreManagementService.AddGenreAsync(genre);
            return CreatedAtAction(nameof(GetGenreById), new { id = addedGenre.Id }, addedGenre);
        }

        /// <summary>
        /// Retrieves a genre by its ID.
        /// </summary>
        /// <param name="id">The ID of the genre to retrieve.</param>
        /// <returns>The requested genre.</returns>
        /// <response code="200">The genre was found.</response>
        /// <response code="404">The genre with the specified ID was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenreDto>> GetGenreById(int id)
        {
            var genre = await _genreManagementService.GetGenreByIdAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            var genreDto = _mapper.Map<GenreDto>(genre);
            return genreDto;
        }
    }
}
