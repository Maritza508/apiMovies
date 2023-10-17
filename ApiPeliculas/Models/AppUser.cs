using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Models
{
    public class AppUser : IdentityUser
    {
        //Añadimos los campos que necesitamos 
        public string Name { get; set; }
    }
}
