using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos.User
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string UserName { get; set; }
      
        [Required(ErrorMessage = "La contraseña es obligatorio")]
        public string Password { get; set; }
    }
}
