
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public enum TypeClasification { Siete, Trece, Dieciseis, Diesiocho }
        public TypeClasification Clasification { get; set; }
        public DateTime CreationDate { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
