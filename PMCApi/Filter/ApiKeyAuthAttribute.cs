using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Filter
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute,IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "Authorization";
        public async  Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var Potentialkey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = configuration.GetValue<string>("ApiKey");
            if (!apiKey.Equals(Potentialkey))
            {
                context.Result=new UnauthorizedResult();
                return;
            }
            await next();

        }

    }
}