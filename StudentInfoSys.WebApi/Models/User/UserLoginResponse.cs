namespace StudentInfoSys.WebApi.Models.User
{
    public class UserLoginResponse
    {
        public required string Message { get; set; }
        public required string Token { get; set; }
    }
}
