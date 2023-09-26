using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.User;
using ApiPeliculas.Repository.IRepository;
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
        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        }
        public ICollection<User> GetAll()
        {
           return _context.Users.OrderByDescending(p => p.Name).ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(p => p.Id == id);
        }

        public bool IsUnique(string user)
        {
            var userExist = _context.Users.FirstOrDefault(u => u.Name == user);
            if (userExist == null)
            {
                return true;
            }
            return false;
        }

        public async Task<User> Register(UserRegisterDto userRegisterDto)
        {
            var encryptPassword = Getmd5(userRegisterDto.Password);
            User user = new User()
            {
                UserName = userRegisterDto.UserName,
                Name = userRegisterDto.Name,
                Password = encryptPassword,
                Role = userRegisterDto.Role
            };

            _context.Add(user);
            await _context.SaveChangesAsync();
            user.Password = encryptPassword;
            return user;
        }


        //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
        public static string Getmd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
                resp += data[i].ToString("x2").ToLower();
            return resp;
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            var encryptPassword = Getmd5(userLoginDto.Password);
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == userLoginDto.UserName.ToLower() && u.Password == encryptPassword);

            if (user is null)
            {
                return new UserLoginResponseDto()
                {
                    Token = "",
                    User = null
                };
            }

            //Existe usuario, procesamos login 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(3),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            UserLoginResponseDto userLoginResponseDto = new UserLoginResponseDto()
            {
                Token = tokenHandler.WriteToken(token),
                User = user
            };
            return userLoginResponseDto;
        }

    }
}
