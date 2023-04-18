using AutoMapper;
using VideoClub.Common.DTOs.Genres;
using VideoClub.Common.DTOs.Movies;
using VideoClub.Data.Models;

namespace VideoClub.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Movie, MovieDto>()
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre)));

            CreateMap<CreateMovieDto, Movie>()
                .ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.GenreIds.Select(id => new MovieGenre { GenreId = id })));

            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<Genre, CreateGenreDto>().ReverseMap();
            CreateMap<Genre, UpdateGenreDto>().ReverseMap();

            CreateMap<Movie, MovieDto>()
    .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.MovieGenres.Select(mg => mg.Genre.Name).ToList()));
        }
    }
}
