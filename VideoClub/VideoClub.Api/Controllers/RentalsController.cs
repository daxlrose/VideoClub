using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VideoClub.Common.DTOs.Rentals;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;

namespace VideoClub.Api.Controllers
{
    /// <summary>
    /// Controller responsible for managing rental records.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalManagementService _rentalManagementService;
        private readonly IMapper _mapper;

        public RentalsController(IRentalManagementService rentalManagementService,
            IMapper mapper)
        {
            _rentalManagementService = rentalManagementService;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new rental record for a specific movie and user.
        /// </summary>
        /// <param name="rentalDto">The rental data transfer object containing the movie and user details.</param>
        /// <returns>The created rental record.</returns>
        /// <response code="201">Returns the created rental record.</response>
        /// <response code="400">Invalid model.</response>
        // POST: api/Rentals
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Rental>> CreateRental([FromBody] CreateRentalDto rentalDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rental = _mapper.Map<Rental>(rentalDto);

            var createdRental = await _rentalManagementService.AddRentalAsync(rental);
            return CreatedAtAction(nameof(GetRentalById), new { id = createdRental.Id }, createdRental);
        }

        /// <summary>
        /// Retrieves a specific rental record by its ID.
        /// </summary>
        /// <param name="id">The ID of the rental record to retrieve.</param>
        /// <returns>The rental record if found, NotFound otherwise.</returns>
        /// <response code="200">Returns the rental record.</response>
        /// <response code="404">If the rental record is not found.</response>
        // GET: api/Rentals/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RentalDto>> GetRentalById(int id)
        {
            var rental = await _rentalManagementService.GetRentalByIdAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            var rentalDto = _mapper.Map<RentalDto>(rental);

            return Ok(rentalDto);
        }
    }
}
