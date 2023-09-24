using ApiPeliculas.Models;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IMovieRepository
    {
        ICollection<Movie> GetAll();
        Movie GetById(int id);
        bool MovieExists(string name);
        bool MovieExists(int id);
        bool CreateMovie(Movie movie); 
        bool UpdateMovie(Movie movie); 
        bool DeleteMovie(Movie movie);

        //Métodos para buscar peliculas según la categoria y buscar pelicula por nombre 
        ICollection<Movie> GetMoviesByCategory(int idCategory);
        ICollection<Movie> GetMoviesByNameOrDescription(string name);
        bool Save();
    }
}
