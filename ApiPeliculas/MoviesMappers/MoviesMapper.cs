using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Category;
using AutoMapper;

namespace ApiPeliculas.MoviesMappers
{
    public class MoviesMapper : Profile
    {
        public MoviesMapper()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
        }
    }
}
