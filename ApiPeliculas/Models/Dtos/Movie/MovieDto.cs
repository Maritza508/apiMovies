using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models.Dtos.Movie
{
    public class MovieDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        public string ImagePath { get; set; }
        [Required(ErrorMessage = "La descripción es obligatorio")]
        public string Description { get; set; }
        [Required(ErrorMessage = "La duración es obligatorio")]
        public int Duration { get; set; } 
        public enum TypeClasification { Siete, Trece, Dieciseis, Diesiocho }
        public TypeClasification Clasification { get; set; }
        public DateTime CreationDate { get; set; }
        public int CategoryId { get; set; }        
    }
}
