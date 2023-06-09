﻿using AutoMapper;
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

        /// <summary>
        /// Updates an existing genre.
        /// </summary>
        /// <param name="id">The ID of the genre to update.</param>
        /// <param name="genreDto">The updated genre information.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        /// <response code="200">The genre was successfully updated.</response>
        /// <response code="400">Invalid input.</response>
        /// <response code="404">The genre with the specified ID was not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] UpdateGenreDto genreDto)
        {
            var genre = await _genreManagementService.GetGenreByIdAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            _mapper.Map(genreDto, genre);
            await _genreManagementService.UpdateGenreAsync(genre);

            return Ok();
        }

        /// <summary>
        /// Deletes a genre by its ID.
        /// </summary>
        /// <param name="id">The ID of the genre to delete.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        /// <response code="200">The genre was successfully deleted.</response>
        /// <response code="404">The genre with the specified ID was not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genre = await _genreManagementService.GetGenreByIdAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            await _genreManagementService.DeleteGenreAsync(genre);

            return Ok();
        }

        /// <summary>
        /// Retrieves all genres.
        /// </summary>
        /// <returns>A list of all genres.</returns>
        /// <response code="200">A list of all genres.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll()
        {
            var genres = await _genreManagementService.GetAllGenresAsync();
            var genreDtos = _mapper.Map<IEnumerable<GenreDto>>(genres);
            return Ok(genreDtos);
        }
    }
}
