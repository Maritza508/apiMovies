using ApiPeliculas.Models;

namespace ApiPeliculas.Repository.IRepository
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetAll();
        Category GetById(int id);
        bool CategoryExists(string name);
        bool CategoryExists(int id);
        bool CreateCategory(Category category); 
        bool UpdateCategory(Category category); 
        bool DeleteCategory(Category category);
        bool Save();
    }
}
