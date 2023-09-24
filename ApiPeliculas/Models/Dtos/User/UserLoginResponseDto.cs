namespace ApiPeliculas.Models.Dtos.User
{
    public class UserLoginResponseDto
    {
        public Models.User User { get; set; }
        public string Token { get; set; }
    }
}
