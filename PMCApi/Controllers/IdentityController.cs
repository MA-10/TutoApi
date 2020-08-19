using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using Test.Requests;
using Test.Response;
using Test.Services;

namespace Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public IdentityController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
           _mapper = mapper;
        }

        [HttpGet("Users")]
        //[Authorize(Policy = "UsersView")]
        public async Task<IActionResult> GetUser()
        {
            return Ok(await _identityService.GetUsersAsync());
        }
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(UserLoginRegisterRequest))]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserLoginRegisterRequest request)
        {
            try
            {
                var authResponse = await _identityService.RegisterAsync(request.Email, request.Password);
            
                if (!authResponse.Success)
                {
                    return BadRequest(_mapper.Map<AuthFailedResponse>(authResponse));
                }
                return Ok(_mapper.Map<AuthSuccessResponse>(authResponse));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(UserLoginRegisterRequest))]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRegisterRequest request)
        {
            try
            {
                
                //if (!ModelState.IsValid)
                //{
                //    return BadRequest(new AuthFailedResponse
                //    {
                //        Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                //    });
                //}
                var authResponse = await _identityService.LoginAsync(request.Email, request.Password);
                if (!authResponse.Success)
                {
                    return BadRequest(_mapper.Map<AuthFailedResponse>(authResponse));
                }
                return Ok(_mapper.Map<AuthSuccessResponse>(authResponse));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

                if (!authResponse.Success)
                {
                    return BadRequest(_mapper.Map<AuthFailedResponse>(authResponse));
                }
                

                return Ok(_mapper.Map<AuthSuccessResponse>(authResponse));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }
}