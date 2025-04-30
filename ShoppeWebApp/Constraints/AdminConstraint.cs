using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ShoppeWebApp.Constraints
{
    public class AdminConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            // Logic to check if the area is "Admin"
            return values[routeKey]?.ToString()?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}