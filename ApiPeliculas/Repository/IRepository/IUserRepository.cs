using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.User;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<User> GetAll();
        User GetById(int id);
        bool IsUnique(string user);
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        Task<User> Register(UserRegisterDto userRegisterDto);
    }
}
