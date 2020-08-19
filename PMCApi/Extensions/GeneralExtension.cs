using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Test.Extensions
{
    public static  class GeneralExtension
    {
        public static string GetUserId ( this HttpContext httpContext)
        {
            if (httpContext.User==null)
            {
                return string.Empty;
            }

            return httpContext.User.Claims.Select(x => x.Type.ToLower() == "id").ToString();
        }
        


    }
}