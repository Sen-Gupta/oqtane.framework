using Microsoft.AspNetCore.Http;

using Oqtane.Managers;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Oqtane.Security
{
    public class SwaggerSecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _swaggerPaths = new[] { "/swagger", "/swagger/index.html", "/swagger/v1/swagger.json" };

        public SwaggerSecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserManager userManger, IAliasRepository aliasRepository)
        {
            // Check if the request is for swagger resources
            if (_swaggerPaths.Any(p => context.Request.Path.StartsWithSegments(p, System.StringComparison.OrdinalIgnoreCase)))
            {
                // Get the current user if authenticated
                User user = null;
                if (context.User.Identity.IsAuthenticated)
                {
                    int userId = int.Parse(context.User.Claims.First(item => item.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                    user = userManger.GetUser(userId, 0);
                }

                // Get the current alias
                //var alias = aliasRepository.GetAlias(context.Request.Host.Value);

                // Check if user is authorized to access Swagger (must be an Admin)
                if (!UserSecurity.IsAuthorized(user, RoleNames.Admin))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync("<html><body><h1>Unauthorized</h1><p>You must be logged in as an administrator to access the API documentation.</p></body></html>");
                    return;
                }
            }

            await _next(context);
        }
    }
}
