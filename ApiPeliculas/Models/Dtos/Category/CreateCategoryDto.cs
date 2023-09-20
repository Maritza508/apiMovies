using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos.Category
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El número máximo de caracteres es 100!")]
        public string Name { get; set; }
    }
}
