using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repository.Implementations
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;
        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool MovieExists(string name)
        {
            bool exists = _context.Movies.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
            return exists;
        }

        public bool MovieExists(int id)
        {
            return _context.Movies.Any(c => c.Id == id);
        }

        public bool CreateMovie(Movie movie)
        {
            movie.CreationDate = DateTime.Now;
            _context.Movies.Add(movie);
            return Save();
        }

        public bool DeleteMovie(Movie movie)
        {
            _context.Movies.Remove(movie);
            return Save();
        }

        public ICollection<Movie> GetAll()
        {
            return _context.Movies.OrderBy(c => c.Name).ToList();
        }

        public Movie GetById(int id)
        {
            return _context.Movies.FirstOrDefault(c => c.Id == id);
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateMovie(Movie movie)
        {
            movie.CreationDate = DateTime.Now;
            _context.Movies.Update(movie);
            return Save();
        }

        public ICollection<Movie> GetMoviesByCategory(int idCategory)
        {
            return _context.Movies.Include(ca => ca.Category).Where(ca => ca.CategoryId == idCategory).ToList();
        }

        public ICollection<Movie> GetMoviesByNameOrDescription(string name)
        {
            IQueryable<Movie> query = _context.Movies;
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name) || p.Description.Contains(name));
            }
            return query.ToList();
        }
    }
}
