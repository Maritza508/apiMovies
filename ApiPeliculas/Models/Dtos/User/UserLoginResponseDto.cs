namespace ApiPeliculas.Models.Dtos.User
{
    public class UserLoginResponseDto
    {
        public UserDataDto User { get; set; }
        public string Token { get; set; }
    }
}
