using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Test.Response;
using Test.Models;
using ErrorModel = Test.Models.ErrorModel;

namespace Test.Filter
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                var errorsInModelStat = context.ModelState.Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key,kvp => kvp.Value.Errors.Select(x=> x.ErrorMessage))
                    .ToArray();
                var errorResponse = new ErrorResponse();
                foreach (var error in errorsInModelStat)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new ErrorModel
                        {
                            FieldName = error.Key,
                            ErrorMessage = subError
                        };
                        errorResponse.Errors.Add(errorModel);
                    }
                }
                context.Result=new BadRequestObjectResult(errorResponse);
                return;

            }
            await next();
        }
    }
}