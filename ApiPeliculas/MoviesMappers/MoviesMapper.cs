using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Category;
using ApiPeliculas.Models.Dtos.Movie;
using AutoMapper;

namespace ApiPeliculas.MoviesMappers
{
    public class MoviesMapper : Profile
    {
        public MoviesMapper()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Movie, MovieDto>().ReverseMap();
        }
    }
}
