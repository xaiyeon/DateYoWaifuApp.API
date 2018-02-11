namespace DateYoWaifuApp.API.Dtos
{
    // No need to check Model State because we are looking directly in database for a match

    public class UserForLoginDto
    {
        public string Username { get; set;}
        public string Password { get; set;}
        public string Email { get; set;}
    }
}