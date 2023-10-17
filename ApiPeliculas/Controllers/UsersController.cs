using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos.Category;
using ApiPeliculas.Models.Dtos.Role;
using ApiPeliculas.Models.Dtos.User;
using ApiPeliculas.Repository.Implementations;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using XAct.Users;

namespace ApiPeliculas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        protected ResponseApi _responseApi;

        public UsersController(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            this._responseApi = new();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]    
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var users = _userRepository.GetAll();
            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                usersDto.Add(_mapper.Map<UserDto>(user));
            }
            return Ok(usersDto);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpGet]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public IActionResult GetAllRoles()
        //{
        //    var roles = _userRepository.GetAllRoles();
        //    var rolesDto = new List<RoleDto>();
        //    foreach (var rol in roles)
        //    {
        //        rolesDto.Add(_mapper.Map<RoleDto>(rol));
        //    }

        //    return Ok(rolesDto);
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetUserById(string id)
        {
            var user = _userRepository.GetById(id);
            if (user is null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            bool validateUserName = _userRepository.IsUnique(userRegisterDto.UserName);
            if (!validateUserName)
            {
                _responseApi.HttpStatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_responseApi);
            }

            var user = await _userRepository.Register(userRegisterDto);
            if (user is null)
            {
                _responseApi.HttpStatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("Error en el registro");
                return BadRequest(_responseApi);
            }

            _responseApi.HttpStatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            return Ok(_responseApi); 
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var responseLogin = await _userRepository.Login(userLoginDto);
            if (responseLogin.User is null || string.IsNullOrEmpty(responseLogin.Token))
            {
                _responseApi.HttpStatusCode = HttpStatusCode.BadRequest;
                _responseApi.IsSuccess = false;
                _responseApi.ErrorMessages.Add("El nombre de usuario o contraseña son incorrectos");
                return BadRequest(_responseApi);
            }

     
            _responseApi.HttpStatusCode = HttpStatusCode.OK;
            _responseApi.IsSuccess = true;
            _responseApi.Result = responseLogin;
            return Ok(_responseApi);
        }
    }
}
