using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos.User
{
    public class UserRegisterDto
    {

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Name { get; set; }
        [Required(ErrorMessage = "La contraseña es obligatorio")]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
