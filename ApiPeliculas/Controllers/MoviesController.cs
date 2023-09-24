using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Movie;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMapper mapper, IMovieRepository movieRepository)
        {
            _mapper = mapper;
            _movieRepository = movieRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var movies = _movieRepository.GetAll();
            var moviesDto = new List<MovieDto>();
            foreach (var movie in movies)
            {
                moviesDto.Add(_mapper.Map<MovieDto>(movie));
            }
            return Ok(moviesDto);
        }

        [HttpGet("{id:int}", Name = "GetByIdMovie")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetByIdMovie(int id)
        {
            var movie = _movieRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = _mapper.Map<MovieDto>(movie);
            return Ok(movieDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(MovieDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] MovieDto movieDto)
        {
            if (!ModelState.IsValid || movieDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_movieRepository.MovieExists(movieDto.Name))
            {
                ModelState.AddModelError("", "La pelicula ya existe");
                return StatusCode(404, ModelState);
            }

            var movie = _mapper.Map<Movie>(movieDto);
            if (!_movieRepository.CreateMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro {movieDto.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetByIdMovie", new { id = movie.Id }, movie);
        }

        [HttpPatch("{id:int}", Name = "UpdatePatchMovie")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePatchMovie(int id, [FromBody] MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var movie = _mapper.Map<Movie>(movieDto);
            if (!_movieRepository.UpdateMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {movieDto.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetByIdMovie", new { id = movie.Id }, movie);
        }

        [HttpDelete("{id:int}", Name = "DeleteMovie")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteMovie(int id)
        {
            if (!_movieRepository.MovieExists(id))
            {
                return NotFound();
            }
            var movie = _movieRepository.GetById(id);
            if (!_movieRepository.DeleteMovie(movie))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro {id}");
                return StatusCode(500, ModelState);
            }

            return new JsonResult(new { success = true });
        }

        [HttpGet("GetMovieByCategory/{id:int}")]
        public IActionResult GetMovieByCategory(int id)
        {
            var movies = _movieRepository.GetMoviesByCategory(id);
            if (movies == null)
            {
                return NotFound();
            }
            var movieList = new List<MovieDto>();
            foreach (var movieItem in movies)
            {
                movieList.Add(_mapper.Map<MovieDto>(movieItem));
            }
            return Ok(movieList);
        }

        [HttpGet("Search")]
        public IActionResult Search(string name)
        {
            try
            {
                var movies = _movieRepository.GetMoviesByNameOrDescription(name.Trim());
                if (movies.Any())
                {
                    return Ok(movies);
                }
                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos");
            }
        }
    }
}
