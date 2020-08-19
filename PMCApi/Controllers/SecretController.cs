using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Test.Filter;

namespace Test.Controllers
{
    [ApiKeyAuth]
    public class SecretController : ControllerBase
    {

        [HttpGet("secret")]
        public IActionResult GetSecret()
        {
            return Ok("I have no secret ");
        }
        
    }
}