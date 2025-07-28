﻿namespace StudentInfoSys.Business.Operations.User.Dtos
{
    public class UserRegisterDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime BirthDate { get; set; }
    }
}