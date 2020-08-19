using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Swashbuckle.Swagger.Model;
using Test.Extensions;
using Test.Services;
using Test.Helpers;
using Test.Extensions;
using Test.Models;
using Test.Requests.Query;
using Test.Response;

namespace Test.Controllers
{
    /// <response code="200">Operation has succeeded</response>
    /// <response code="401">Authorization Failed</response>
    /// <response code ="500">Error in Configuration</response>
    /// /// <response code="404"> Ammar Not Found</response>
    [Route("api/[controller]")]
    [GenericControllerNameAttribute]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenericController<T> : Controller where T : class, IApplicationEntity
    {
        private IApplicationRepository<T> _repository;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private IEnumerable<Claim> userClaims;
        private readonly IUriService _uriService;

        public GenericController(IApplicationRepository<T> repository, IMapper mapper, UserManager<IdentityUser> userManager, IUriService uriService, IEnumerable<Claim> userClaims)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
            _uriService = uriService;
            this.userClaims = userClaims;
        }
        
        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] String[] fields = null,
        [FromQuery] String[] values = null,
        [FromQuery] String[] types = null,
        [FromQuery] String[] operators = null,
        [FromQuery] PaginationQuery paginationQuery = null)
        {
            var parameters = GenerateSearchParameters(fields, values, types, operators);
            var pagination = _mapper.Map<PaginationFilter>(paginationQuery);
            var list = await _repository.GetAllPageableAsync(pagination, parameters);
            if (pagination == null || pagination.PageNumber < 1 || pagination.PageSize < 1)
                return Ok(new PagedResponse<T>(list));
            var paginationResponse = PaginationHelpers.CreatePaginatedResponse(_uriService, pagination, list);
            return Ok(paginationResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Find(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var record = await _repository.GetAsync(id);
            if (record == null)
                return NotFound();

            return Ok(record);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] T record)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();
                var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "id")?.Value;
                if (user != null) record.CreatedBy = Guid.Parse(user);
                _repository.Create(record);
                if (await _repository.SaveAsync() == 0)
                    return BadRequest();

                return CreatedAtAction("Find", new {id = record.Id}, record);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] T record)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (id != record.Id)
                return BadRequest("Wrong Id Request");
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "id")?.Value;
            if (userId != null) record.ModifiedBy = Guid.Parse(userId);
            var item = await _repository.GetAsync(id);
            if (item == null) return NotFound();
            Test.Helpers.ReflectionHelpers.CopyProperties(item, record);

            _repository.Update(item);
            if (await _repository.SaveAsync() == 0)
                return BadRequest();

            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            _repository.Delete(id);
            if (await _repository.SaveAsync() == 0)
                return BadRequest();

            return NoContent();
        }
        private SearchParameter[] GenerateSearchParameters(string[] fields, string[] values, string[] types,
            string[] operators)
        {
            Type[] enumerable = types.Select(x => Type.GetType($"System.{x}")

            ).ToArray();
            SearchParameter[] parameters = GenerateParams(fields, values, operators, enumerable).ToArray();
            return parameters;
        }
        private IEnumerable<SearchParameter> GenerateParams(string[] fields, String[] values, string[] operators,
            Type[] types)
        {
            for (int i = 0; i < fields?.Length; i++)
            {
                yield return CreateParam(fields[i], Convert
                    .ChangeType(values[i], types[i]), operators[i]);
            }
        }
        private SearchParameter CreateParam(string field, object value, string @operator)
            => new SearchParameter(field, value, @operator);


    }
}
