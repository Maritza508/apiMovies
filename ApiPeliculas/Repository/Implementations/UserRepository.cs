using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Role;
using ApiPeliculas.Models.Dtos.User;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private string secretKey;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public ICollection<AppUser> GetAll()
        {
           return _context.AppUsers.OrderBy(p => p.UserName).ToList();
        }

        public ICollection<RoleDto> GetAllRoles()
        {
            var roleDtos = _roleManager.Roles
                .OrderBy(p => p.Name)
                .Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    // Agrega otras propiedades de RoleDto según sea necesario
                })
                .ToList();

            return roleDtos;
        }

        public AppUser GetById(string id)
        {
            return _context.AppUsers.FirstOrDefault(p => p.Id == id);
        }

        public bool IsUnique(string user)
        {
            var userExist = _context.AppUsers.FirstOrDefault(u => u.UserName == user);
            if (userExist == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UserDataDto> Register(UserRegisterDto userRegisterDto)
        {
            AppUser user = new AppUser()
            {
                UserName = userRegisterDto.UserName,
                Email = userRegisterDto.UserName,   
                NormalizedEmail = userRegisterDto.UserName.ToUpper(),
                Name = userRegisterDto.Name,
            };

            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);
            if (result.Succeeded)
            {
                if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult()) 
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                    await _userManager.AddToRoleAsync(user, "Admin");
                    var returnUser = _context.AppUsers.FirstOrDefault(p => p.UserName == userRegisterDto.UserName);
                return _mapper.Map<UserDataDto>(returnUser);
            }
            return new UserDataDto();
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            var user = _context.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == userLoginDto.UserName.ToLower());
            bool isValidPassword = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);

            if (user is null || isValidPassword == false)
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }

            //Existe usuario, procesamos login 
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddMinutes(3),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            UserLoginResponseDto userLoginResponseDto = new UserLoginResponseDto()
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDataDto>(user)
            };
            return userLoginResponseDto;
        }

    }
}
