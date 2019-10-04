using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace Ambient
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AmbientContextAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (new Op(context.HttpContext.Request.GetDisplayUrl()))
            using (new Cancellation(context.HttpContext.RequestAborted))
                await next();
        }
    }
}
