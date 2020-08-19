using System.ComponentModel.DataAnnotations;

namespace Test.Requests
{
    public class UserLoginRegisterRequest
    {   
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}