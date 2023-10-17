    using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Role;
using ApiPeliculas.Models.Dtos.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IUserRepository
    {
        ICollection<AppUser> GetAll();
        ICollection<RoleDto> GetAllRoles();
        AppUser GetById(string id);
        bool IsUnique(string user);
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        Task<UserDataDto> Register(UserRegisterDto userRegisterDto);
    }
}
