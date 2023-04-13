using AutoMapper;
using VideoClub.Common.DTOs.Movies;
using VideoClub.Data.Models;

namespace VideoClub.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, CreateMovieDto>().ReverseMap();
        }
    }
}
