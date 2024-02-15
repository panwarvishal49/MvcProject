using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
namespace MvcProject
{
    public class SetSessionGlobally : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var value = context.HttpContext.Session.GetString("UserName");
            if(value == null)
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"Controller","Action"},
                        {"Action","login"}
                    });
            }
            base.OnActionExecuting(context);
        }
    }
}
