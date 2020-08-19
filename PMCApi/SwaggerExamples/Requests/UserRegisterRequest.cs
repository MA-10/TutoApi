using Swashbuckle.AspNetCore.Filters;
using Test.Requests;

namespace Test.SwaggerExamples.Requests
{
    public class UserRegisterRequestExample : IExamplesProvider<UserLoginRegisterRequest>
    {
        public UserLoginRegisterRequest GetExamples()
        {
            return new UserLoginRegisterRequest
            {
                Email = "mah@gmail.com",
                Password = "Mahmoud#15935728"
            };
        }
    }
}