using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Category;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IMapper mapper, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var categories = _categoryRepository.GetAll();
            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
            {
                categoriesDto.Add(_mapper.Map<CategoryDto>(category));
            }
            return Ok(categoriesDto);
        }
        
        [HttpGet("{id:int}", Name = "GetById")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            var category = _categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }

            var categoyDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoyDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] CreateCategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }        
            if(categoryDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_categoryRepository.CategoryExists(categoryDto.Name))
            {
                ModelState.AddModelError("", "La categoría ya existe");
                return StatusCode(404, ModelState);
            }
            
            var category = _mapper.Map<Category>(categoryDto);
            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("", $"Algo salió mal guardando el registro {categoryDto.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetById", new { id = category.Id }, category);
        }
        
        [HttpPatch("{id:int}", Name = "UpdatePatch")]
        [ProducesResponseType(201, Type = typeof(CategoryDto))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePatch(int id, [FromBody] CategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }        
            if(categoryDto == null || id != categoryDto.Id)
            {
                return BadRequest(ModelState);
            }
            
            var category = _mapper.Map<Category>(categoryDto);
            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("", $"Algo salió mal actualizando el registro {categoryDto.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetById", new { id = category.Id }, category);
        }
        
        [HttpDelete("{id:int}", Name = "Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Delete(int id)
        {
            if (!_categoryRepository.CategoryExists(id))
            {
                return NotFound();
            }
            var category = _categoryRepository.GetById(id);
            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("", $"Algo salió mal borrando el registro {id}");
                return StatusCode(500, ModelState);
            }

            return new JsonResult(new { success = true }); 
        }
    }
}
