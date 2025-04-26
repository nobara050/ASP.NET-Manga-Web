using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace WebTruyenTranh.Data
{
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated ||
                !user.IsInRole("admin"))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new { controller = "Account", action = "Login", area = "" }));
            }
        }
    }
}

