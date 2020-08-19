using System.Collections.Generic;
using Test.Models;
namespace Test.Response
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}