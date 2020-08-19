using AutoMapper;
using Test.Models;
using Test.Requests;
using Test.Requests.Query;
using Test.Response;

namespace Test.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AuthenticationResult, AuthSuccessResponse>();
            CreateMap<AuthenticationResult, AuthFailedResponse>();
            CreateMap<PaginationQuery, PaginationFilter>();
            
        }
    }
}