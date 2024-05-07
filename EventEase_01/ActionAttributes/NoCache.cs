
using Microsoft.AspNetCore.Mvc.Filters;

namespace EventEase_01.ActionAttributes
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var response = context.HttpContext.Response;
            response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
            response.Headers.Expires = "-1";
            response.Headers.Pragma = "no-cache";
            base.OnResultExecuting(context);
        }
    }
}
