using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoClub.Common.DTOs.Movies;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;

namespace VideoClub.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing movie-related actions.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieManagementService _movieManagementService;
        private readonly IMapper _mapper;

        public MoviesController(IMovieManagementService movieManagementService,
            IMapper mapper)
        {
            _movieManagementService = movieManagementService;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new movie.
        /// </summary>
        /// <param name="movie">The movie to add.</param>
        /// <returns>A newly created movie.</returns>
        /// <response code="201">A new movie was created.</response>
        /// <response code="400">Invalid input.</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Movie>> AddMovie([FromBody] CreateMovieDto movieDto)
        {
            var movie = _mapper.Map<Movie>(movieDto);
            var addedMovie = await _movieManagementService.AddMovieAsync(movie);
            return CreatedAtAction(nameof(GetMovieById), new { id = addedMovie.Id }, addedMovie);
        }

        /// <summary>
        /// Retrieves a movie by its ID.
        /// </summary>
        /// <param name="id">The ID of the movie to retrieve.</param>
        /// <returns>The requested movie.</returns>
        /// <response code="200">The movie was found.</response>
        /// <response code="404">The movie with the specified ID was not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Movie>> GetMovieById(int id)
        {
            var movie = await _movieManagementService.GetMovieByIdAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }
    }
}
